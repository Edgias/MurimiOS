using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Application.Common.Imports;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.Export;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;

public sealed class ExportHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ExportQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ExportQuery message, CancellationToken cancellationToken) => ExportAsync(message.type, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> ExportAsync(string type, IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess, CancellationToken ct)
    {
        if (!TryType(type, out MasterDataImportType target)) return Bad("master_data_type_invalid", TypeMessage());
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(ct);
        IQueryable<Farm> farmsQuery = db.Farms.AsNoTracking();
        if (!scope.IsUnrestricted) farmsQuery = farmsQuery.Where(x => scope.FarmIds!.Contains(x.Id));
        Farm[] farms = await farmsQuery.OrderBy(x => x.Code).ToArrayAsync(ct);
        Guid[] farmIds = farms.Select(x => x.Id).ToArray();
        Dictionary<Guid, string> codes = farms.ToDictionary(x => x.Id, x => x.Code);
        IEnumerable<IEnumerable<string?>> rows = target switch
        {
            MasterDataImportType.Farms => await ExportFarmsAsync(db, farms, ct),
            MasterDataImportType.Fields => await ExportFieldsAsync(db, farmIds, codes, ct),
            MasterDataImportType.Livestock => await ExportLivestockAsync(db, farmIds, codes, ct),
            MasterDataImportType.Inventory => await ExportInventoryAsync(db, farmIds, codes, ct),
            MasterDataImportType.Suppliers => ExportSuppliers(await db.HostedSupplierAccounts.AsNoTracking().OrderBy(x => x.SupplierCode).ToArrayAsync(ct)),
            MasterDataImportType.OpeningBalances => await ExportOpeningBalancesAsync(db, farmIds, codes, ct),
            MasterDataImportType.FinanceTransactions => await ExportFinanceAsync(db, farmIds, codes, ct),
            _ => throw new UnreachableException()
        };
        return UseCaseResults.File(Encoding.UTF8.GetBytes(MasterDataCsv.Write(rows)), "text/csv; charset=utf-8",
            $"murimios-{Slug(target)}-{DateTime.UtcNow:yyyyMMdd}.csv");
    }


    private static async Task<IEnumerable<IEnumerable<string?>>> ExportFarmsAsync(IPlatformDbContext db, Farm[] farms, CancellationToken ct)
    {
        Guid[] nodeIds = farms.Select(x => x.OrganisationNodeId).ToArray();
        Dictionary<Guid, Guid?> parents = await db.OrganisationNodes.AsNoTracking().Where(x => nodeIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.ParentId, ct);
        return Header(MasterDataImportType.Farms).Concat(farms.Select(x => (IEnumerable<string?>)
            [x.Name, x.Code, x.CountryCode, x.TimeZone, parents.GetValueOrDefault(x.OrganisationNodeId)?.ToString()]));
    }

    private static async Task<IEnumerable<IEnumerable<string?>>> ExportFieldsAsync(IPlatformDbContext db, Guid[] farms,
        Dictionary<Guid, string> codes, CancellationToken ct)
    {
        var rows = await (from field in db.FarmFields.AsNoTracking() join boundary in db.FieldBoundaryVersions.AsNoTracking()
            on field.Id equals boundary.FieldId where farms.Contains(field.FarmId) && boundary.ValidTo == null
            orderby field.FarmId, field.Name select new { field.FarmId, field.Name, boundary.Boundary, boundary.ValidFrom, boundary.Source }).ToArrayAsync(ct);
        var writer = new WKTWriter();
        return Header(MasterDataImportType.Fields).Concat(rows.Select(x => (IEnumerable<string?>)
            [codes[x.FarmId], x.Name, writer.Write(x.Boundary), x.ValidFrom.ToString("O"), x.Source]));
    }

    private static async Task<IEnumerable<IEnumerable<string?>>> ExportLivestockAsync(IPlatformDbContext db, Guid[] farms,
        Dictionary<Guid, string> codes, CancellationToken ct)
    {
        LivestockSubject[] rows = await db.LivestockSubjects.AsNoTracking().Where(x => farms.Contains(x.FarmId)).OrderBy(x => x.Name).ToArrayAsync(ct);
        return Header(MasterDataImportType.Livestock).Concat(rows.Select(x => (IEnumerable<string?>)
            [codes[x.FarmId], x.Kind.ToString(), x.Name, x.Species, x.OfficialIdentifier]));
    }

    private static async Task<IEnumerable<IEnumerable<string?>>> ExportInventoryAsync(IPlatformDbContext db, Guid[] farms,
        Dictionary<Guid, string> codes, CancellationToken ct)
    {
        var rows = await (from input in db.AgriculturalInputs.AsNoTracking() join lot in db.InputStockLots.AsNoTracking()
            on input.Id equals lot.InputId where farms.Contains(input.FarmId) orderby input.Name, lot.LotCode
            select new { input.FarmId, input.Name, input.Category, input.DisplayUnit, lot.LotCode, lot.ReceivedOn,
                lot.ExpiresOn, lot.EnteredInitialQuantity }).ToArrayAsync(ct);
        return Header(MasterDataImportType.Inventory).Concat(rows.Select(x => (IEnumerable<string?>)
            [codes[x.FarmId], x.Name, x.Category, x.DisplayUnit, x.LotCode, x.ReceivedOn.ToString("O"),
                x.ExpiresOn?.ToString("O"), x.EnteredInitialQuantity.ToString(CultureInfo.InvariantCulture)]));
    }

    private static IEnumerable<IEnumerable<string?>> ExportSuppliers(HostedSupplierAccount[] rows) =>
        Header(MasterDataImportType.Suppliers).Concat(rows.Select(x => (IEnumerable<string?>)
            [x.SupplierCode, x.Name, x.ContactName, x.Email, x.Phone]));

    private static async Task<IEnumerable<IEnumerable<string?>>> ExportOpeningBalancesAsync(IPlatformDbContext db, Guid[] farms,
        Dictionary<Guid, string> codes, CancellationToken ct)
    {
        FinanceOpeningBalance[] rows = await db.FinanceOpeningBalances.AsNoTracking().Where(x => farms.Contains(x.FarmId))
            .OrderBy(x => x.AsOf).ThenBy(x => x.AccountCode).ToArrayAsync(ct);
        return Header(MasterDataImportType.OpeningBalances).Concat(rows.Select(x => (IEnumerable<string?>)
            [codes[x.FarmId], x.AccountCode, x.AccountName, x.Direction.ToString(), x.Amount.ToString(CultureInfo.InvariantCulture),
                x.Currency, x.AsOf.ToString("O"), x.Reference]));
    }

    private static async Task<IEnumerable<IEnumerable<string?>>> ExportFinanceAsync(IPlatformDbContext db, Guid[] farms,
        Dictionary<Guid, string> codes, CancellationToken ct)
    {
        FinanceTransaction[] rows = await db.FinanceTransactions.AsNoTracking().Where(x => farms.Contains(x.FarmId))
            .OrderBy(x => x.OccurredAt).ThenBy(x => x.Reference).ToArrayAsync(ct);
        return Header(MasterDataImportType.FinanceTransactions).Concat(rows.Select(x => (IEnumerable<string?>)
            [codes[x.FarmId], x.EntryType.ToString(), x.Category, x.Amount.ToString(CultureInfo.InvariantCulture), x.Currency,
                x.OccurredAt.ToString("O"), x.Reference, x.Counterparty, x.Description]));
    }


    private static IEnumerable<IEnumerable<string?>> Header(MasterDataImportType type) => [MasterDataCsv.TemplateColumns(type)];

    private static MultiPolygon Boundary(string wkt)
    {
        Geometry geometry = new WKTReader(NtsGeometryServices.Instance).Read(wkt);
        MultiPolygon result = geometry switch
        {
            Polygon polygon => geometry.Factory.CreateMultiPolygon([polygon]),
            MultiPolygon multi => multi,
            _ => throw new ArgumentException("boundary_wkt must contain a Polygon or MultiPolygon.")
        };
        result.SRID = 4326;
        if (result.IsEmpty || !result.IsValid || result.Coordinates.Any(x => x.X is < -180 or > 180 || x.Y is < -90 or > 90))
            throw new ArgumentException("boundary_wkt must be a valid WGS84 polygon.");
        return result;
    }

    private static bool TryType(string value, out MasterDataImportType type) =>
        Enum.TryParse(value.Replace("-", ""), true, out type) && Enum.IsDefined(type);

    private static string TypeMessage() => "Use Farms, Fields, Livestock, Inventory, Suppliers, OpeningBalances, or FinanceTransactions.";

    private static string Slug(MasterDataImportType type) => type.ToString().ToLowerInvariant();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
