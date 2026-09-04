using System.Net;
using System.Text;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using IcarAdeCredentials = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeCredentials;
using IcarAdeExchangeResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeExchangeResult;
using IcarAdeOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeOptions;
using IcarAdeExchangeAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock.IcarAdeExchangeAdapter;

public sealed class LivestockExchangeTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 2, 10, 0, 0, TimeSpan.Zero);
    private const string Hash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Fact]
    public void Movement_profile_is_effective_dated_and_requires_both_deadlines()
    {
        Assert.Throws<ArgumentException>(() => new LivestockRegulatoryProfile(Guid.NewGuid(), "move", 1,
            "Movement", LivestockRegulatoryExchangeType.Movement, "GB-SCT", "cattle", "scoteid",
            "1.7.9", "application/xml", 72, null, new DateOnly(2026, 1, 1), null,
            "https://www.gov.scot/", false));

        var profile = new LivestockRegulatoryProfile(Guid.NewGuid(), "gb-sct-cattle-movement", 2,
            "Scotland cattle movement", LivestockRegulatoryExchangeType.Movement, "GB-SCT", "cattle",
            "scoteid", "1.7.9", "application/xml", 72, 48, new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31), "https://www.gov.scot/", true);

        Assert.Equal(2, profile.ProfileVersion);
        Assert.Equal(48, profile.RegisterDeadlineHours);
        Assert.True(profile.AppliesOn(Now));
        Assert.False(profile.AppliesOn(new DateOnly(2027, 1, 1)));
    }

    [Fact]
    public void Pending_exchange_can_be_acknowledged_exactly_once()
    {
        var submission = new LivestockExchangeSubmission(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), LivestockRegulatoryExchangeType.MilkRecording, "icar-ade",
            "1.5.0", "application/json", "{}", Hash, Now, "recorder", Now.AddDays(1));

        submission.MarkSubmitted(Now.AddMinutes(1), "PENDING-1");
        submission.Accept(Now.AddMinutes(2), "ACCEPTED-1");

        Assert.Equal(LivestockExchangeSubmissionStatus.Accepted, submission.Status);
        Assert.Equal("ACCEPTED-1", submission.ExternalReference);
        Assert.Throws<InvalidOperationException>(() => submission.Reject(Now.AddMinutes(3), "LATE", "Too late"));
    }

    [Fact]
    public async Task Icar_adapter_sends_idempotency_and_maps_async_acceptance()
    {
        Guid submissionId = Guid.NewGuid();
        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new StringContent("{\"status\":\"pending\",\"submissionId\":\"ADE-42\"}", Encoding.UTF8, "application/json")
        };
        var handler = new CapturingHandler(response);
        var adapter = new IcarAdeExchangeAdapter(new HttpClient(handler));

        IcarAdeExchangeResult result = await adapter.SubmitAsync(
            new IcarAdeOptions(new Uri("https://recording.example.test/ade"), "ORG-1", "1.5.0"),
            new IcarAdeCredentials("token", null), submissionId, "{\"messageType\":\"milkingObservation\"}", default);

        Assert.True(result.Pending);
        Assert.False(result.Accepted);
        Assert.Equal("ADE-42", result.ExternalReference);
        Assert.Equal(submissionId.ToString("N"), handler.IdempotencyKey);
        Assert.Equal("Bearer", handler.AuthorizationScheme);
    }

    [Fact]
    public void Icar_parser_maps_validation_rejection_and_retriable_server_failure()
    {
        IcarAdeExchangeResult rejected = IcarAdeExchangeAdapter.ParseResponse(
            "{\"status\":\"rejected\",\"errorCode\":\"ANIMAL_ID\",\"message\":\"Unknown animal\"}",
            200, true, Hash);
        IcarAdeExchangeResult retry = IcarAdeExchangeAdapter.ParseResponse("{}", 503, false, Hash);

        Assert.Equal("ANIMAL_ID", rejected.ErrorCode);
        Assert.False(rejected.Retriable);
        Assert.True(retry.Retriable);
    }

    private sealed class CapturingHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public string? IdempotencyKey { get; private set; }
        public string? AuthorizationScheme { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            IdempotencyKey = request.Headers.GetValues("Idempotency-Key").Single();
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            return Task.FromResult(response);
        }
    }
}
