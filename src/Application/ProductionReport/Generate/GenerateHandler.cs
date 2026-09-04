using EdgIQ.MurimiOS.Application.Common.Reporting;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ReportExportRecord;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProductionReport.Generate;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ReportExportRecord = global::EdgIQ.MurimiOS.Domain.ReportExportRecord.ReportExportRecord;

public sealed class GenerateHandler(UseCaseContext httpContext, IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, ITenantContext tenantContext, ProductionReportPdfBuilder pdfBuilder, TimeProvider timeProvider) : IRequestHandler<GenerateQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GenerateQuery message, CancellationToken cancellationToken) => GenerateAsync(message.cropZoneId, httpContext, db, scopeAccess, tenantContext, pdfBuilder, timeProvider, cancellationToken);

    private static async Task<UseCaseResult> GenerateAsync(
        Guid cropZoneId,
        UseCaseContext httpContext,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        ITenantContext tenantContext,
        ProductionReportPdfBuilder pdfBuilder,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        var context = await (
            from zone in db.CropZones.AsNoTracking()
            join season in db.GrowingSeasons.AsNoTracking() on zone.SeasonId equals season.Id
            join field in db.FarmFields.AsNoTracking() on zone.FieldId equals field.Id
            join farm in db.Farms.AsNoTracking() on season.FarmId equals farm.Id
            where zone.Id == cropZoneId && field.FarmId == farm.Id
                && (scope.IsUnrestricted || scope.FarmIds!.Contains(farm.Id))
            select new
            {
                Farm = farm.Name,
                Field = field.Name,
                Season = season.Name,
                CropZone = zone.Name,
                zone.Crop,
                zone.Variety,
                zone.AreaHa,
                zone.BoundaryVersionId
            }).SingleOrDefaultAsync(cancellationToken);

        if (context is null)
        {
            return UseCaseResults.NotFound();
        }

        ProductionOperationRow[] operations = await db.FieldOperations
            .AsNoTracking()
            .Where(operation => operation.CropZoneId == cropZoneId && operation.SupersededById == null)
            .OrderBy(operation => operation.OccurredAt)
            .Select(operation => new ProductionOperationRow(
                operation.OccurredAt,
                operation.OperationType,
                operation.Notes))
            .ToArrayAsync(cancellationToken);

        InputApplicationRow[] inputApplications = await (
            from movement in db.InventoryMovements.AsNoTracking()
            join operation in db.FieldOperations.AsNoTracking()
                on movement.OperationId equals (Guid?)operation.Id
            join stockLot in db.InputStockLots.AsNoTracking() on movement.StockLotId equals stockLot.Id
            join input in db.AgriculturalInputs.AsNoTracking() on stockLot.InputId equals input.Id
            where operation.CropZoneId == cropZoneId && movement.Type == InventoryMovementType.Issue
            orderby movement.OccurredAt
            select new InputApplicationRow(
                movement.OccurredAt,
                input.Name,
                stockLot.LotCode,
                movement.Quantity,
                input.Unit,
                movement.Notes))
            .ToArrayAsync(cancellationToken);

        HarvestLotRow[] harvestLots = await db.HarvestLots
            .AsNoTracking()
            .Where(lot => lot.CropZoneId == cropZoneId)
            .OrderBy(lot => lot.HarvestedAt)
            .Select(lot => new HarvestLotRow(
                lot.HarvestedAt,
                lot.LotCode,
                lot.Quantity,
                lot.Unit,
                lot.Notes))
            .ToArrayAsync(cancellationToken);

        EvidenceRow[] evidence = await db.EvidenceAttachments
            .AsNoTracking()
            .Where(item => item.CropZoneId == cropZoneId)
            .OrderBy(item => item.CapturedAt)
            .Select(item => new EvidenceRow(
                item.Id,
                item.CapturedAt,
                item.FileName,
                item.ContentType,
                item.ByteSize,
                item.Latitude,
                item.Longitude,
                item.AccuracyMetres,
                item.Sha256))
            .ToArrayAsync(cancellationToken);

        Guid reportId = Guid.NewGuid();
        DateTimeOffset generatedAt = timeProvider.GetUtcNow();
        byte[] pdf = pdfBuilder.Build(new ProductionReportData(
            reportId,
            generatedAt,
            context.Farm,
            context.Field,
            context.Season,
            context.CropZone,
            context.Crop,
            context.Variety,
            context.AreaHa,
            context.BoundaryVersionId,
            operations,
            inputApplications,
            harvestLots,
            evidence));

        string sha256 = Convert.ToHexStringLower(SHA256.HashData(pdf));
        string? requestedBy = httpContext.User.Principal.FindFirst("sub")?.Value;
        db.ReportExports.Add(new ReportExportRecord(
            reportId,
            cropZoneId,
            "Crop zone production and evidence",
            "pdf",
            sha256,
            generatedAt,
            requestedBy));

        string eventPayload = JsonSerializer.Serialize(new
        {
            reportId,
            cropZoneId,
            format = "pdf",
            sha256,
            generatedAt,
            requestedBy
        });
        db.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(),
            tenantContext.TenantId,
            "Report.Generated",
            eventPayload,
            generatedAt));
        await db.SaveChangesAsync(cancellationToken);

        httpContext.Response["X-Report-Id"] = reportId.ToString();
        httpContext.Response["X-Content-SHA256"] = sha256;
        httpContext.Response["X-Content-Type-Options"] = "nosniff";
        httpContext.Response["Cache-Control"] = "no-store";

        string fileName = $"{Slug(context.Farm)}-{Slug(context.CropZone)}-production-{generatedAt:yyyyMMdd}.pdf";
        return UseCaseResults.File(pdf, "application/pdf", fileName, enableRangeProcessing: false);
    }


    private static string Slug(string value)
    {
        string result = new(value
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsAsciiLetterOrDigit(character) ? character : '-')
            .ToArray());
        string slug = string.Join('-', result.Split('-', StringSplitOptions.RemoveEmptyEntries));
        return slug.Length == 0 ? "record" : slug;
    }

}
