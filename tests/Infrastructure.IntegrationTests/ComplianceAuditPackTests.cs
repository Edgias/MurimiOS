using System.Text;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;


using EdgIQ.MurimiOS.Infrastructure.Storage.Compliance;
using EdgIQ.MurimiOS.Application.Common.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;
using ComplianceAuditPackFormat = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackFormat;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ComplianceAuditPackOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ComplianceAuditPackOptions;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using ComplianceAuditPackSignatureService = global::EdgIQ.MurimiOS.Infrastructure.Storage.Compliance.ComplianceAuditPackSignatureService;

public sealed class ComplianceAuditPackTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-09-01T17:00:00Z");
    private const string Hash = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    [Fact]
    public void Export_freezes_revision_scope_digests_and_signature_metadata()
    {
        var export = Export();

        Assert.Equal(2, export.Revision);
        Assert.Equal(1, export.ContentSchemaVersion);
        Assert.Equal("HMAC-SHA256", export.SignatureAlgorithm);
        Assert.Equal(Hash, export.ArtifactSha256);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ComplianceAuditPackExport(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), 0, 1, "Farm scope", false, ComplianceAuditPackFormat.Zip,
            "key", 10, 2, 0, Hash, Hash, "HMAC-SHA256", "key-1", "c2ln", "{}", Now, "auditor"));
    }

    [Fact]
    public void Signatures_are_fixed_time_verifiable_and_historical_keys_can_be_retained()
    {
        string oldKey = Convert.ToBase64String(Enumerable.Range(1, 32).Select(x => (byte)x).ToArray());
        string newKey = Convert.ToBase64String(Enumerable.Range(33, 32).Select(x => (byte)x).ToArray());
        byte[] manifest = Encoding.UTF8.GetBytes("{\"schemaVersion\":1}");
        var oldSigner = new ComplianceAuditPackSignatureService(Options.Create(new ComplianceAuditPackOptions
        {
            SigningKeyBase64 = oldKey, SigningKeyId = "key-1"
        }));
        string signature = oldSigner.Sign(manifest);
        var rotatedSigner = new ComplianceAuditPackSignatureService(Options.Create(new ComplianceAuditPackOptions
        {
            SigningKeyBase64 = newKey,
            SigningKeyId = "key-2",
            VerificationKeys = new Dictionary<string, string> { ["key-1"] = oldKey }
        }));

        Assert.True(rotatedSigner.Verify("key-1", manifest, signature));
        manifest[1] ^= 1;
        Assert.False(rotatedSigner.Verify("key-1", manifest, signature));
    }

    [Fact]
    public async Task Completed_export_records_are_append_only()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        ComplianceAuditPackExport export = Export();
        db.ComplianceAuditPackExports.Add(export);
        await db.SaveChangesAsync();
        db.ComplianceAuditPackExports.Remove(export);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private static ComplianceAuditPackExport Export() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        Guid.NewGuid(), 2, 1, "Farm, fields, crop records, evidence, and corrective actions for the 2026 audit.",
        true, ComplianceAuditPackFormat.Zip, "tenant/exports/audit.zip", 1_024, 9, 2, Hash, Hash,
        "HMAC-SHA256", "key-1", "c2lnbmF0dXJl", "{}", Now, "auditor");

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
