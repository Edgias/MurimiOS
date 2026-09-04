using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

namespace EdgIQ.MurimiOS.Domain.UnitTests;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrCommodity = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCommodity;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrRiskConclusion = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskConclusion;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
using EudrScreeningResult = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrScreeningResult;
using EudrStatementStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrStatementStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;

public sealed class EudrWorkflowTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-31T04:00:00Z");
    private static readonly string Digest = new('a', 64);

    [Theory]
    [InlineData(100, 0, 0, 95, EudrScreeningResult.Clear)]
    [InlineData(94, 0, 0, 95, EudrScreeningResult.ReviewRequired)]
    [InlineData(100, 0, 0, 79, EudrScreeningResult.ReviewRequired)]
    [InlineData(100, 0.01, 0, 99, EudrScreeningResult.PotentialDeforestation)]
    [InlineData(100, 0, 0.01, 99, EudrScreeningResult.PotentialDeforestation)]
    public void Screening_result_is_deterministic(decimal coverage, decimal loss, decimal overlap,
        decimal confidence, EudrScreeningResult expected)
    {
        Assert.Equal(expected, EudrDeforestationScreening.DeriveResult(coverage, loss, overlap, confidence));
    }

    [Fact]
    public void Screening_requires_data_that_covers_the_regulation_cutoff()
    {
        Assert.Throws<ArgumentException>(() => new EudrDeforestationScreening(Guid.NewGuid(), Guid.NewGuid(),
            "copernicus", "Global Forest Cover", "2026.1", new DateOnly(2021, 1, 1), new DateOnly(2026, 8, 1),
            100, 0, 0, 99, "scene-1", null, "{}", Digest, Now, "analyst"));
    }

    [Fact]
    public void Case_follows_screen_assess_assemble_export_submit_sequence()
    {
        EudrDueDiligenceCase dueDiligenceCase = Case();
        Assert.Throws<InvalidOperationException>(() => dueDiligenceCase.RecordAssessment(Now));
        dueDiligenceCase.StartScreening();
        dueDiligenceCase.RecordAssessment(Now);
        dueDiligenceCase.MarkReady(Now.AddMinutes(1));
        dueDiligenceCase.MarkExported(Now.AddMinutes(2));
        dueDiligenceCase.MarkSubmitted(Now.AddMinutes(3), "EUIS-DDS-123");
        Assert.Equal(EudrCaseStatus.Submitted, dueDiligenceCase.Status);
        Assert.Equal("EUIS-DDS-123", dueDiligenceCase.SubmissionReference);
        Assert.Throws<InvalidOperationException>(() => dueDiligenceCase.Cancel(Now.AddMinutes(4), "too late"));
    }

    [Fact]
    public void Negligible_decision_requires_legality_confirmation()
    {
        Assert.Throws<ArgumentException>(() => new EudrRiskDecision(Guid.NewGuid(), Guid.NewGuid(), 1,
            EudrRiskConclusion.Negligible, false, "No detected loss", null, "{}", "{}", Digest, Now, "reviewer"));
        var decision = new EudrRiskDecision(Guid.NewGuid(), Guid.NewGuid(), 1, EudrRiskConclusion.Negligible,
            true, "No detected loss and legality documents verified", null, "{}", "{}", Digest, Now, "reviewer");
        Assert.True(decision.LegalProductionConfirmed);
    }

    [Fact]
    public void Mitigated_decision_requires_mitigation_evidence()
    {
        Assert.Throws<ArgumentException>(() => new EudrRiskDecision(Guid.NewGuid(), Guid.NewGuid(), 1,
            EudrRiskConclusion.MitigatedToNegligible, true, "Reviewed alert", null, "{}", "{}", Digest, Now, "reviewer"));
    }

    [Fact]
    public void Assembled_statement_payload_is_hash_bound_and_has_ordered_lifecycle()
    {
        var statement = new EudrDueDiligenceStatement(Guid.NewGuid(), Guid.NewGuid(), 1,
            "murimios.eudr.dds.v1", "{\"case\":\"DDS-1\"}", Digest, Now, "operator");
        Assert.Throws<InvalidOperationException>(() => statement.MarkSubmitted(Now, "DDS-REF"));
        statement.MarkExported(Now);
        statement.MarkSubmitted(Now.AddMinutes(1), "DDS-REF");
        Assert.Equal(EudrStatementStatus.Submitted, statement.Status);
    }

    [Fact]
    public void Roles_receive_separated_eudr_permissions()
    {
        Assert.Contains(PlatformPermissions.EudrExport, PlatformPermissions.ForRole(TenantRole.Buyer));
        Assert.Contains(PlatformPermissions.EudrAssess, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.Contains(PlatformPermissions.EudrScreen, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.DoesNotContain(PlatformPermissions.EudrManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static EudrDueDiligenceCase Case() => new(Guid.NewGuid(), Guid.NewGuid(), "DDS-2026-001",
        EudrCommodity.Coffee, "Green coffee beans", "090111", 10_000, "kg", "ZW",
        new DateOnly(2026, 3, 1), new DateOnly(2026, 8, 1), "Example Operator Ltd", "EU-OP-123",
        "1 Market Street, Harare", "European Union", Now, "operator");
}
