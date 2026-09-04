using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.AddRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;

public sealed class AddRequirementHandler(IPlatformDbContext db) : IRequestHandler<AddRequirementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddRequirementCommand message, CancellationToken cancellationToken) => AddRequirementAsync(message.standardId, message.controlId, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> AddRequirementAsync(Guid standardId, Guid controlId, CreateComplianceRequirementRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        bool draft = await db.ComplianceStandardVersions.AnyAsync(x => x.Id == standardId && x.Status == ComplianceStandardStatus.Draft, ct);
        bool controlExists = await db.ComplianceControlPoints.AnyAsync(x => x.Id == controlId && x.StandardVersionId == standardId, ct);
        if (!controlExists) return UseCaseResults.NotFound();
        if (!draft) return Conflict("standard_immutable", "Published standard versions cannot be changed.");
        try
        {
            var requirement = new ComplianceEvidenceRequirement(Id(request.Id), controlId, request.Code, request.Name,
                request.EvidenceType, request.Required, request.MinimumCount, request.MaximumAgeDays, request.Instructions);
            db.ComplianceEvidenceRequirements.Add(requirement); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/compliance/standards/{standardId}/controls/{controlId}/requirements/{requirement.Id}", requirement);
        }
        catch (ArgumentException e) { return Bad("requirement_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
