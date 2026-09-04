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
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using ScotEidCredentials = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidCredentials;
using ScotEidMovementSubmission = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidMovementSubmission;
using ScotEidOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidOptions;
using ScotEidSubmissionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidSubmissionResult;
using ScotEidMovementAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock.ScotEidMovementAdapter;

public sealed class LivestockStatutoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);
    private const string Hash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Fact]
    public void Statutory_document_snapshot_can_only_be_finalized_once()
    {
        LivestockStatutoryMovementDocument document = Document();

        document.Accept(Now.AddMinutes(1), Now.AddMinutes(2), "SME-123");

        Assert.Equal(StatutoryMovementDocumentStatus.Accepted, document.Status);
        Assert.Equal("SME-123", document.ExternalReference);
        Assert.Throws<InvalidOperationException>(() => document.Reject(Now.AddMinutes(3), Now.AddMinutes(4), "E1", "duplicate"));
    }

    [Fact]
    public void Rejected_document_retains_machine_readable_reason()
    {
        LivestockStatutoryMovementDocument document = Document();
        document.Reject(Now.AddMinutes(1), Now.AddMinutes(2), "ANIMAL_ID", "Identifier is invalid.");

        Assert.Equal(StatutoryMovementDocumentStatus.Rejected, document.Status);
        Assert.Equal("ANIMAL_ID", document.RejectionCode);
        Assert.Null(document.ExternalReference);
    }

    [Fact]
    public void Crop_feed_transfer_canonicalizes_mass_and_preserves_source_lineage()
    {
        Guid sourceId = Guid.NewGuid();
        Guid feedBatchId = Guid.NewGuid();
        var transfer = new CropFeedTransfer(Guid.NewGuid(), Guid.NewGuid(), CropFeedSourceType.HarvestLot,
            sourceId, feedBatchId, Guid.NewGuid(), 2.20462m, "lb", Now, "stock.manager", "home-grown maize");

        Assert.Equal(1.0000m, decimal.Round(transfer.Quantity, 4));
        Assert.Equal("kg", transfer.Unit);
        Assert.Equal(sourceId, transfer.SourceId);
        Assert.Equal(feedBatchId, transfer.FeedBatchId);
    }

    [Fact]
    public void ScotEid_envelope_contains_required_metadata_and_escapes_values()
    {
        var options = new ScotEidOptions(new Uri("https://api.test.scoteid.com/api"), "MurimiOS", "1.2", "1.7.9");
        var credentials = new ScotEidCredentials("user", "password", "key<&>");
        var movement = new ScotEidMovementSubmission("UK123", "12/345/6789", "98/765/4321", "Shed & Field",
            new DateOnly(2026, 8, 30), new DateOnly(2026, 8, 31), "ref<1>");

        string xml = ScotEidMovementAdapter.BuildEnvelope(options, credentials, movement, Now);

        Assert.Contains("SMECreateCattleMovementsRequest", xml);
        Assert.Contains("ApplicationKey", xml);
        Assert.Contains("key&lt;&amp;&gt;", xml);
        Assert.Contains("Shed &amp; Field", xml);
        Assert.DoesNotContain("password", xml);
    }

    [Fact]
    public async Task ScotEid_adapter_maps_acceptance_and_uses_basic_authentication()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<Envelope><Body><Result><Status>Success</Status><MovementReference>SME-42</MovementReference></Result></Body></Envelope>", Encoding.UTF8, "text/xml")
        };
        var handler = new CapturingHandler(response);
        var adapter = new ScotEidMovementAdapter(new HttpClient(handler));
        var options = new ScotEidOptions(new Uri("https://api.test.scoteid.com/api"), "MurimiOS", "1.0", "1.7.9");
        var credentials = new ScotEidCredentials("user", "secret", "key");
        var movement = new ScotEidMovementSubmission("UK123", "12/345/6789", "98/765/4321", "Main",
            new DateOnly(2026, 8, 30), new DateOnly(2026, 8, 31), "ref");

        ScotEidSubmissionResult result = await adapter.SubmitAsync(options, credentials, movement, default);

        Assert.True(result.Accepted);
        Assert.Equal("SME-42", result.MovementReference);
        Assert.Equal("Basic", handler.AuthorizationScheme);
        Assert.Equal(64, result.RequestSha256.Length);
    }

    [Fact]
    public void ScotEid_parser_maps_validation_errors_as_permanent()
    {
        ScotEidSubmissionResult result = ScotEidMovementAdapter.ParseResponse(
            "<Result><Status>Error</Status><Error Code=\"CPH\" Message=\"Invalid holding\" /></Result>", 200, Hash);

        Assert.False(result.Accepted);
        Assert.False(result.Retriable);
        Assert.Equal("CPH", result.ErrorCode);
        Assert.Equal("Invalid holding", result.ErrorMessage);
    }

    private static LivestockStatutoryMovementDocument Document() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        Guid.NewGuid(), "GB-SCT", "scoteid", "1.7.9", "DOC-1", "{}", Hash, Now, "manager",
        Now.AddDays(3), Now.AddHours(48));

    private sealed class CapturingHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public string? AuthorizationScheme { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            return Task.FromResult(response);
        }
    }
}
