using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingSeverity = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingSeverity;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class ComplianceWorkflowTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T12:00:00Z");

    [Fact]
    public void Standard_version_has_ordered_publication_lifecycle()
    {
        var standard = Standard();
        Assert.Throws<InvalidOperationException>(() => standard.Retire(Now));
        standard.Publish(Now, "auditor");
        Assert.Equal(ComplianceStandardStatus.Published, standard.Status);
        Assert.Throws<InvalidOperationException>(() => standard.Publish(Now, "auditor"));
        standard.Retire(Now.AddDays(1));
        Assert.Equal(ComplianceStandardStatus.Retired, standard.Status);
    }

    [Fact]
    public void Evidence_requirements_enforce_required_minimum_and_age()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ComplianceEvidenceRequirement(Guid.NewGuid(),
            Guid.NewGuid(), "CP1-E1", "Field record", ComplianceEvidenceType.Record, true, 0, null, null));
        var requirement = new ComplianceEvidenceRequirement(Guid.NewGuid(), Guid.NewGuid(), "CP1-E1",
            "Field record", ComplianceEvidenceType.Record, true, 2, 30, "Upload signed records");
        Assert.Equal(2, requirement.MinimumCount);
        Assert.Equal(30, requirement.MaximumAgeDays);
    }

    [Fact]
    public void Assessment_and_evaluation_lifecycles_are_ordered()
    {
        var assessment = new ComplianceAssessment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "A-001",
            new DateOnly(2026, 9, 1), "auditor", null);
        Assert.Throws<InvalidOperationException>(() => assessment.Submit(Now));
        assessment.Start(Now);
        var evaluation = new ComplianceControlEvaluation(Guid.NewGuid(), assessment.Id, Guid.NewGuid(),
            ComplianceEvaluationResult.Conformant, 95, "1.0", "auditor", Now, null, "[]");
        Assert.Equal(95, evaluation.ScorePercent);
        assessment.Submit(Now.AddHours(1)); assessment.Complete(95, true, Now.AddHours(2));
        Assert.True(assessment.Conforms);
    }

    [Fact]
    public void Corrective_action_requires_independent_verification_before_closure()
    {
        var finding = new ComplianceFinding(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            ComplianceFindingSeverity.Major, "Missing record", "A spray record is absent",
            new DateOnly(2026, 9, 30), Now);
        var action = new ComplianceCorrectiveAction(Guid.NewGuid(), finding.Id, "Reconstruct and approve record",
            "farm-manager", new DateOnly(2026, 9, 15));
        finding.MarkActionPending(); action.Start(); action.Submit(Now.AddDays(1), "Record attached");
        finding.MarkVerificationPending();
        Assert.Throws<InvalidOperationException>(() => finding.MarkActionPending());
        action.Verify(Now.AddDays(2), "auditor"); finding.Close(Now.AddDays(2), "auditor");
        Assert.Equal(ComplianceFindingStatus.Closed, finding.Status);
    }

    [Fact]
    public async Task Evaluation_and_evidence_links_are_append_only_in_application_model()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var evaluation = new ComplianceControlEvaluation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            ComplianceEvaluationResult.Conformant, 100, "1", "auditor", Now, null, "[]");
        db.ComplianceControlEvaluations.Add(evaluation); await db.SaveChangesAsync();
        db.ComplianceControlEvaluations.Remove(evaluation);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Compliance_permissions_separate_assessment_and_verification()
    {
        Assert.Contains(PlatformPermissions.ComplianceManage, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.ComplianceVerify, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.ComplianceVerify, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.DoesNotContain(PlatformPermissions.ComplianceManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static ComplianceStandardVersion Standard() => new(Guid.NewGuid(), Guid.NewGuid(), "GLOBALGAP-IFA",
        "Integrated Farm Assurance", "GlobalG.A.P.", "v6", 1, new DateOnly(2026, 1, 1), null, null);
    private sealed class TestTenantContext(Guid tenantId) : ITenantContext { public Guid TenantId { get; } = tenantId; public bool IsResolved => true; }
}
