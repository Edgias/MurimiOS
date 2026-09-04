using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordRiskDecision;
using EudrRiskConclusion = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskConclusion;
public sealed record RecordEudrRiskDecisionRequest(Guid? Id, EudrRiskConclusion Conclusion,
    bool LegalProductionConfirmed, string Rationale, JsonElement? Mitigation, JsonElement LegalityEvidence);
