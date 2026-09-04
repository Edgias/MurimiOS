using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface IComplianceAuditPackGenerator
{
    Task<ComplianceAuditPackArtifact> GenerateAsync(Guid exportId, Guid tenantId, Guid farmId, Guid assessmentId, int revision, string scopeStatement, bool includeEvidenceFiles, string actor, DateTimeOffset generatedAt, CancellationToken ct);
}
