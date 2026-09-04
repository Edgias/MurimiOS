using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.AddControl;
using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class AddControlHandler(IPlatformDbContext db) : IRequestHandler<AddControlCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddControlCommand message, CancellationToken cancellationToken) => AddControlAsync(message.standardId, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> AddControlAsync(Guid standardId, CreateComplianceControlRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        ComplianceStandardVersion? standard = await db.ComplianceStandardVersions.SingleOrDefaultAsync(x => x.Id == standardId, ct);
        if (standard is null) return UseCaseResults.NotFound();
        if (standard.Status != ComplianceStandardStatus.Draft) return Conflict("standard_immutable", "Published standard versions cannot be changed.");
        try
        {
            var control = new ComplianceControlPoint(Id(request.Id), standardId, request.Code, request.Title, request.Description,
                request.Applicability, request.Weight, request.Critical, request.EvaluatorKey, request.EvaluatorVersion, request.SortOrder);
            db.ComplianceControlPoints.Add(control); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/compliance/standards/{standardId}/controls/{control.Id}", control);
        }
        catch (ArgumentException e) { return Bad("control_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
