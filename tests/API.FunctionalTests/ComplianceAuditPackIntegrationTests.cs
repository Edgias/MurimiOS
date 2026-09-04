using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class ComplianceAuditPackIntegrationTests
{
    [IntegrationFact]
    public async Task Completed_assessment_exports_a_signed_versioned_and_verifiable_zip()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid marker = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);

        HttpResponseMessage farmCreated = await client.PostAsJsonAsync("/api/v1/farms", new
        {
            name = "Audit Pack Integration Farm",
            code = $"AP{marker:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        });
        Assert.Equal(HttpStatusCode.Created, farmCreated.StatusCode);
        Guid farmId = await IdAsync(farmCreated);

        HttpResponseMessage standardCreated = await client.PostAsJsonAsync("/api/v1/compliance/standards", new
        {
            seriesId = Guid.NewGuid(),
            code = $"AUD-{marker:N}"[..20],
            name = "Integration controlled audit standard",
            scheme = "Integration",
            versionLabel = "2026.1",
            revision = 1,
            effectiveFrom = new DateOnly(2026, 1, 1)
        });
        Assert.Equal(HttpStatusCode.Created, standardCreated.StatusCode);
        Guid standardId = await IdAsync(standardCreated);

        HttpResponseMessage controlCreated = await client.PostAsJsonAsync(
            $"/api/v1/compliance/standards/{standardId}/controls", new
            {
                code = "CTRL-001",
                title = "Controlled records are complete",
                description = "Verify the scoped controlled farm records.",
                applicability = 1,
                weight = 100,
                critical = true,
                evaluatorKey = "integration.manual",
                evaluatorVersion = "1.0.0",
                sortOrder = 1
            });
        Assert.Equal(HttpStatusCode.Created, controlCreated.StatusCode);
        Guid controlId = await IdAsync(controlCreated);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PostAsJsonAsync($"/api/v1/compliance/standards/{standardId}/publish", new { })).StatusCode);

        HttpResponseMessage assessmentCreated = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/compliance/assessments", new
            {
                standardVersionId = standardId,
                assessmentCode = $"ASS-{marker:N}"[..20],
                scheduledOn = new DateOnly(2026, 9, 1),
                auditor = "integration-auditor",
                scope = new { productionYear = 2026, sites = new[] { farmId } }
            });
        Assert.Equal(HttpStatusCode.Created, assessmentCreated.StatusCode);
        Guid assessmentId = await IdAsync(assessmentCreated);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PostAsJsonAsync($"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/start", new { })).StatusCode);

        Assert.Equal(HttpStatusCode.Created, (await client.PutAsJsonAsync(
            $"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/controls/{controlId}", new
            {
                result = 1,
                scorePercent = 100,
                notes = "Records verified for integration scope.",
                evidenceAttachmentIds = Array.Empty<Guid>()
            })).StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PostAsJsonAsync($"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/submit", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await client.PostAsJsonAsync($"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/complete", new { })).StatusCode);

        HttpResponseMessage exported = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/audit-packs", new
            {
                scopeStatement = "Farm operations, controlled records, evaluations, evidence and corrective actions for the 2026 certification review.",
                includeEvidenceFiles = true
            });
        Assert.Equal(HttpStatusCode.Created, exported.StatusCode);
        JsonElement metadata = (await JsonDocument.ParseAsync(await exported.Content.ReadAsStreamAsync())).RootElement;
        Guid exportId = metadata.GetProperty("id").GetGuid();
        Assert.Equal(1, metadata.GetProperty("revision").GetInt32());
        Assert.Equal("HMAC-SHA256", metadata.GetProperty("signatureAlgorithm").GetString());

        HttpResponseMessage downloaded = await client.GetAsync(
            $"/api/v1/farms/{farmId}/compliance/audit-packs/{exportId}/download");
        Assert.Equal(HttpStatusCode.OK, downloaded.StatusCode);
        byte[] zipBytes = await downloaded.Content.ReadAsByteArrayAsync();
        Assert.Equal(metadata.GetProperty("artifactSha256").GetString(),
            downloaded.Headers.GetValues("X-Artifact-SHA256").Single());
        using (var archive = new ZipArchive(new MemoryStream(zipBytes), ZipArchiveMode.Read))
        {
            string[] names = archive.Entries.Select(x => x.FullName).ToArray();
            Assert.Contains("manifest.json", names);
            Assert.Contains("signature.json", names);
            Assert.Contains("scope.json", names);
            Assert.Contains("standard.json", names);
            Assert.Contains("assessment.json", names);
            Assert.Contains("evaluations.json", names);
            Assert.Contains("findings-and-actions.json", names);
            Assert.Contains("evidence-index.json", names);
        }

        HttpResponseMessage verification = await client.GetAsync(
            $"/api/v1/farms/{farmId}/compliance/audit-packs/{exportId}/verify");
        Assert.Equal(HttpStatusCode.OK, verification.StatusCode);
        JsonElement verified = (await JsonDocument.ParseAsync(await verification.Content.ReadAsStreamAsync())).RootElement;
        Assert.True(verified.GetProperty("valid").GetBoolean());
        Assert.True(verified.GetProperty("artifactHashValid").GetBoolean());
        Assert.True(verified.GetProperty("manifestHashValid").GetBoolean());
        Assert.True(verified.GetProperty("signatureValid").GetBoolean());

        HttpResponseMessage secondExport = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/audit-packs", new
            {
                scopeStatement = "Second frozen review revision after the first controlled export.",
                includeEvidenceFiles = false
            });
        Assert.Equal(HttpStatusCode.Created, secondExport.StatusCode);
        JsonElement second = (await JsonDocument.ParseAsync(await secondExport.Content.ReadAsStreamAsync())).RootElement;
        Assert.Equal(2, second.GetProperty("revision").GetInt32());
    }

    [IntegrationFact]
    public async Task Audit_pack_export_table_is_forced_rls_and_runtime_cannot_mutate_exports()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select c.relrowsecurity and c.relforcerowsecurity,
                   not has_table_privilege(current_user,'compliance.audit_pack_exports','UPDATE'),
                   not has_table_privilege(current_user,'compliance.audit_pack_exports','DELETE')
            from pg_class c join pg_namespace n on n.oid=c.relnamespace
            where n.nspname='compliance' and c.relname='audit_pack_exports'
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.True(reader.GetBoolean(0));
        Assert.True(reader.GetBoolean(1));
        Assert.True(reader.GetBoolean(2));
    }

    private static async Task<Guid> IdAsync(HttpResponseMessage response)
    {
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetGuid();
    }
}
