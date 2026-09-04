using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SoilSample;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListSoilSamples;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilSampleStatus = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleStatus;

public sealed class ListSoilSamplesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListSoilSamplesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSoilSamplesQuery message, CancellationToken cancellationToken) => ListSoilSamplesAsync(message.farmId, message.fieldId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListSoilSamplesAsync(Guid farmId, Guid? fieldId, SoilSampleStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<SoilSample> query = db.SoilSamples.AsNoTracking().Where(x => x.FarmId == farmId);
        if (fieldId.HasValue) query = query.Where(x => x.FieldId == fieldId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        SoilSample[] samples = await query.OrderByDescending(x => x.SampledAt).Take(10_000).ToArrayAsync(ct);
        Guid[] ids = samples.Select(x => x.Id).ToArray();
        SoilSampleResult[] results = await db.SoilSampleResults.AsNoTracking().Where(x => ids.Contains(x.SampleId))
            .OrderBy(x => x.AnalyteCode).ToArrayAsync(ct);
        return UseCaseResults.Ok(samples.Select(sample => Sample(sample, results.Where(x => x.SampleId == sample.Id))));
    }

    private static object Sample(SoilSample sample, IEnumerable<SoilSampleResult> results) => new
    {
        sample.Id, sample.FarmId, sample.FieldId, sample.FieldBoundaryVersionId, sample.SoilZoneBoundaryVersionId,
        sample.SampleCode, location = new { longitude = sample.Location.X, latitude = sample.Location.Y },
        sample.SampledAt, sample.SampledBy, sample.DepthFromCm, sample.DepthToCm, sample.LaboratoryName,
        sample.LaboratoryReference, sample.Notes, sample.Status, sample.SubmittedAt, sample.AnalysedAt,
        sample.RejectedAt, sample.RejectionReason, results
    };

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
