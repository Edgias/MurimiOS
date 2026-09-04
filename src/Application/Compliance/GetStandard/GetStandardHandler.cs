using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.GetStandard;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class GetStandardHandler(IPlatformDbContext db) : IRequestHandler<GetStandardQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetStandardQuery message, CancellationToken cancellationToken) => GetStandardAsync(message.standardId, db, cancellationToken);

    private static async Task<UseCaseResult> GetStandardAsync(Guid standardId, IPlatformDbContext db, CancellationToken ct)
    {
        ComplianceStandardVersion? standard = await db.ComplianceStandardVersions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == standardId, ct);
        if (standard is null) return UseCaseResults.NotFound();
        ComplianceControlPoint[] controls = await db.ComplianceControlPoints.AsNoTracking().Where(x => x.StandardVersionId == standardId).OrderBy(x => x.SortOrder).ToArrayAsync(ct);
        Guid[] controlIds = controls.Select(x => x.Id).ToArray();
        ComplianceEvidenceRequirement[] requirements = await db.ComplianceEvidenceRequirements.AsNoTracking().Where(x => controlIds.Contains(x.ControlPointId)).OrderBy(x => x.Code).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { standard, controls = controls.Select(x => new { control = x, requirements = requirements.Where(r => r.ControlPointId == x.Id) }) });
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
