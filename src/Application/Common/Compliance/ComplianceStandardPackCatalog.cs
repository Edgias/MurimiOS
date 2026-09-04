using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
using ComplianceEvidenceType = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceType;

public sealed class ComplianceStandardPackCatalog
{
    public IReadOnlyList<ComplianceStandardPack> Packs { get; } = CreatePacks();
    public ComplianceStandardPack? Find(string key) => Packs.SingleOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

    private static IReadOnlyList<ComplianceStandardPack> CreatePacks() =>
    [
        Pack("globalgap-ifa-v6-smart-fv", "GLOBALGAP-IFA-FV", "IFA v6 Smart readiness – fruit and vegetables", "GLOBALG.A.P.", "v6 Smart", 1, 95,
            new DateOnly(2024, 1, 1), null, "https://globalgap.org/what-we-offer/solutions/ifa-fruit-and-vegetables/",
            C("FV-SITE-01", "Production sites and fields are identified", "Verify that production units have current digital boundaries.", 20, true, "farm.fields.boundaries"),
            C("FV-OPS-02", "Crop operations are recorded", "Verify that crop-zone field operations retain dates, operators and inputs.", 20, true, "crop.operations.records"),
            C("FV-INP-03", "Agricultural inputs are traceable", "Verify input stock and movement records supporting product use.", 20, true, "inputs.traceability"),
            C("FV-TRC-04", "Harvest and dispatch are traceable", "Verify harvest lots and downstream traceability records.", 25, true, "harvest.traceability"),
            E("FV-DOC-05", "Required farm-assurance evidence is current", "Verify current supporting documentation for the assessment.", 15, false, "evidence.minimum", "FV-DOC", "Farm assurance documentation", 1, 365)),

        Pack("codex-haccp-cxc1-2022", "CODEX-HACCP", "Codex HACCP operational readiness", "Codex Alimentarius", "CXC 1-1969 (2022)", 1, 100,
            new DateOnly(2022, 1, 1), null, "https://www.fao.org/fao-who-codexalimentarius/codex-texts/codes-of-practice/en/",
            E("HACCP-PLAN-01", "HACCP plan is documented", "Verify a current documented food-safety plan.", 20, true, "evidence.minimum", "HACCP-PLAN", "HACCP plan", 1, 365),
            E("HACCP-HA-02", "Hazard analysis is documented", "Verify documented biological, chemical and physical hazard analysis.", 20, true, "evidence.minimum", "HACCP-HA", "Hazard analysis", 1, 365),
            E("HACCP-CCP-03", "Critical-control monitoring records exist", "Verify current monitoring records for identified critical controls.", 25, true, "evidence.minimum", "HACCP-CCP", "Critical-control monitoring record", 3, 90),
            C("HACCP-TRC-04", "Product traceability is operational", "Verify harvested product can be followed into dispatch records.", 20, true, "harvest.traceability"),
            E("HACCP-VER-05", "Verification evidence is current", "Verify periodic HACCP verification or review evidence.", 15, false, "evidence.minimum", "HACCP-VER", "HACCP verification", 1, 365)),

        Pack("iso9001-2015-amd1-2024", "ISO-9001", "ISO 9001 quality-management readiness", "ISO", "2015 + Amd 1:2024", 1, 80,
            new DateOnly(2024, 2, 23), null, "https://www.iso.org/standard/88431.html",
            E("QMS-CTX-01", "QMS context and climate relevance are documented", "Verify organizational context, interested parties and climate relevance records.", 20, false, "evidence.minimum", "QMS-CTX", "QMS context review", 1, 365),
            E("QMS-POL-02", "Quality policy and responsibilities are documented", "Verify approved quality policy and assigned responsibilities.", 20, true, "evidence.minimum", "QMS-POL", "Quality policy", 1, 730),
            C("QMS-OBJ-03", "Quality objectives are measured", "Verify current farm KPI or quality-result records.", 20, false, "quality.objectives"),
            E("QMS-AUD-04", "Internal review evidence is current", "Verify internal audit or management-review evidence.", 20, true, "evidence.minimum", "QMS-AUD", "Internal review", 1, 365),
            C("QMS-CAPA-05", "Nonconformities receive corrective action", "Verify overdue corrective actions are controlled.", 20, true, "corrective.actions.controlled")),

        Pack("eudr-2023-1115-consolidated-2025", "EUDR-2023-1115", "EUDR due-diligence readiness", "European Union", "2023/1115 consolidated 2025", 1, 90,
            new DateOnly(2026, 12, 30), "EU", "https://eur-lex.europa.eu/legal-content/EN/ALL/?uri=CELEX:32023R1115",
            C("EUDR-GEO-01", "Production plots have geolocation evidence", "Verify due-diligence plots retain immutable boundary snapshots.", 20, true, "eudr.plot.evidence"),
            C("EUDR-DEF-02", "Plots have deforestation screening", "Verify every plot has post-cutoff screening evidence.", 25, true, "eudr.screening.complete"),
            C("EUDR-RISK-03", "Risk conclusion is negligible", "Verify a current legality-backed negligible or mitigated-to-negligible risk decision.", 25, true, "eudr.risk.negligible"),
            C("EUDR-DDS-04", "Due-diligence statement is assembled", "Verify a versioned, hashed DDS payload exists.", 20, true, "eudr.statement.assembled"),
            C("EUDR-SUB-05", "Submission is recorded", "Verify submission reference and time when the scoped case has reached submission.", 10, false, "eudr.submission.recorded", ComplianceControlApplicability.Conditional))
    ];

    private static ComplianceStandardPack Pack(string key, string code, string name, string scheme, string version,
        int revision, decimal minimumScore, DateOnly effective, string? jurisdiction, string source, params StandardPackControl[] controls) =>
        new(key, code, name, scheme, version, revision, effective, jurisdiction, source,
            "MurimiOS operational readiness mapping; not an official standard text or certification decision.",
            new(minimumScore, true), controls);
    private static StandardPackControl C(string code, string title, string description, decimal weight, bool critical,
        string evaluator, ComplianceControlApplicability applicability = ComplianceControlApplicability.Mandatory) =>
        new(code, title, description, applicability, weight, critical, evaluator, []);
    private static StandardPackControl E(string code, string title, string description, decimal weight, bool critical,
        string evaluator, string requirementCode, string requirementName, int count, int age) =>
        new(code, title, description, ComplianceControlApplicability.Mandatory, weight, critical, evaluator,
            [new(requirementCode, requirementName, ComplianceEvidenceType.Document, true, count, age, "Attach controlled, farm-scoped evidence.")]);
}
