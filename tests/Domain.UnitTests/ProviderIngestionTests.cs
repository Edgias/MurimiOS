using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;

namespace EdgIQ.MurimiOS.Domain.UnitTests;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;

public sealed class ProviderIngestionTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T08:00:00Z");

    [Fact]
    public void Connection_requires_explicit_lifecycle_and_disabled_is_terminal()
    {
        var connection = new IntegrationConnection(Guid.NewGuid(), "Open-Meteo", "Farm weather",
            IntegrationCategory.Weather, "{}", Now, "owner");

        connection.Activate(Now);
        connection.RecordSuccess(Now.AddMinutes(5), 99, Now.AddHours(1));
        connection.Pause();
        connection.Activate(Now.AddMinutes(10));
        connection.Disable(Now.AddMinutes(11), "owner");

        Assert.Equal(IntegrationConnectionStatus.Disabled, connection.Status);
        Assert.Equal(IntegrationHealth.Healthy, connection.Health);
        Assert.Throws<InvalidOperationException>(() => connection.Update("Changed", "{}"));
        Assert.Throws<InvalidOperationException>(() => connection.Activate(Now));
    }

    [Fact]
    public void Credential_rotation_preserves_version_lineage_without_exposing_a_plaintext_field()
    {
        Guid connectionId = Guid.NewGuid();
        var first = new IntegrationCredential(Guid.NewGuid(), connectionId, 1, "vault",
            "protected:v1:ciphertext", "1234567890abcdef", Now, Now.AddDays(30), Now, "owner");
        var second = new IntegrationCredential(Guid.NewGuid(), connectionId, 2, "vault",
            "protected:v2:ciphertext", "abcdef1234567890", Now, Now.AddDays(60), Now, "owner", first.Id);

        first.Supersede(second.Id, Now.AddMinutes(1));

        Assert.Equal(IntegrationCredentialStatus.Superseded, first.Status);
        Assert.Equal(second.Id, first.SupersededByCredentialId);
        Assert.Equal(first.Id, second.PreviousCredentialId);
        Assert.DoesNotContain(typeof(IntegrationCredential).GetProperties(),
            property => property.Name.Contains("Plain", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Feed_checkpoint_and_freshness_metadata_advance_only_after_success()
    {
        var feed = CreateFeed();
        feed.RecordFailure(Now, "timeout", "Provider timed out.", Now.AddMinutes(5));

        Assert.Equal(Now.AddMinutes(5), feed.NextPollAt);
        Assert.Null(feed.CheckpointJson);

        feed.RecordSuccess(Now.AddMinutes(6), Now.AddMinutes(4), "{\"cursor\":\"42\"}");

        Assert.Equal(Now.AddMinutes(21), feed.NextPollAt);
        Assert.Equal(Now.AddMinutes(4), feed.LastObservedAt);
        Assert.Equal("{\"cursor\":\"42\"}", feed.CheckpointJson);
        Assert.Null(feed.LastErrorCode);
    }

    [Fact]
    public void Job_uses_leases_then_retries_and_dead_letters_at_the_attempt_limit()
    {
        var job = CreateJob(maximumAttempts: 2);
        Guid firstLease = Guid.NewGuid();
        job.Claim(firstLease, Now, TimeSpan.FromMinutes(5));
        job.Fail(firstLease, Now.AddMinutes(1), Now.AddMinutes(3), "rate_limited", "Try later.");

        Assert.Equal(IngestionJobStatus.RetryScheduled, job.Status);
        Assert.False(job.IsClaimable(Now.AddMinutes(2)));

        Guid secondLease = Guid.NewGuid();
        job.Claim(secondLease, Now.AddMinutes(3), TimeSpan.FromMinutes(5));
        job.Fail(secondLease, Now.AddMinutes(4), Now.AddMinutes(8), "provider_down", "Unavailable.");

        Assert.Equal(IngestionJobStatus.DeadLettered, job.Status);
        Assert.Equal(2, job.Attempts);
        Assert.NotNull(job.CompletedAt);
        Assert.False(job.IsClaimable(Now.AddHours(1)));
    }

    [Fact]
    public void Expired_job_lease_can_be_reclaimed_but_an_active_lease_cannot()
    {
        var job = CreateJob(maximumAttempts: 3);
        job.Claim(Guid.NewGuid(), Now, TimeSpan.FromMinutes(5));

        Assert.False(job.IsClaimable(Now.AddMinutes(4)));
        Assert.True(job.IsClaimable(Now.AddMinutes(5)));

        Guid replacementLease = Guid.NewGuid();
        job.Claim(replacementLease, Now.AddMinutes(5), TimeSpan.FromMinutes(5));
        job.Succeed(replacementLease, Now.AddMinutes(6), "{\"cursor\":\"complete\"}");

        Assert.Equal(IngestionJobStatus.Succeeded, job.Status);
        Assert.Equal(2, job.Attempts);
        Assert.Equal("{\"cursor\":\"complete\"}", job.ResultCheckpointJson);
    }

    [Fact]
    public void Raw_record_provenance_is_validated_and_decision_is_one_way()
    {
        var record = new IngestionRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "provider-record-42", 1, Now.AddMinutes(-2), Now, Now.AddMinutes(-1),
            new string('a', 64), "{\"temperatureC\":27.4}", IngestionQuality.Valid);

        record.MarkProcessed(Now.AddMinutes(1));

        Assert.Equal(IngestionRecordStatus.Processed, record.Status);
        Assert.Throws<InvalidOperationException>(() => record.Reject(Now.AddMinutes(2), "mapping", "Wrong schema."));
        Assert.Throws<ArgumentException>(() => new IngestionRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), "bad", 1, Now, Now, null, "not-a-digest", "{}", IngestionQuality.Unknown));
    }

    [Fact]
    public void Attempt_validates_provider_cost_and_integration_permissions_are_least_privilege()
    {
        Assert.Throws<ArgumentException>(() => new IngestionAttempt(Guid.NewGuid(), Guid.NewGuid(), 1,
            Now, Now, IngestionAttemptOutcome.Succeeded, 200, 1, 0.01m, null, null, null));

        var attempt = new IngestionAttempt(Guid.NewGuid(), Guid.NewGuid(), 1, Now, Now.AddSeconds(2),
            IngestionAttemptOutcome.Succeeded, 200, 1, 0.01m, "usd", null, null);

        Assert.Equal("USD", attempt.Currency);
        Assert.Contains(PlatformPermissions.IntegrationsOperate, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.IntegrationsRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.IntegrationsManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static IntegrationFeed CreateFeed() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        IntegrationFeedType.Weather, IntegrationTargetType.Farm, null, "Farm weather", "farm-weather-1",
        15, 60, "{}", Now);

    private static IngestionJob CreateJob(int maximumAttempts) => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        IngestionJobKind.Poll, $"poll-{Guid.NewGuid():N}", Now, "worker", Now, maximumAttempts);
}
