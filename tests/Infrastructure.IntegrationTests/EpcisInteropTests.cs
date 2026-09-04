using System.Text;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using EpcisCapturedEnvelope = global::EdgIQ.MurimiOS.Application.Common.Traceability.EpcisCapturedEnvelope;
using EpcisCaptureValidation = global::EdgIQ.MurimiOS.Application.Common.Traceability.EpcisCaptureValidation;
using EpcisInterop = global::EdgIQ.MurimiOS.Application.Common.Traceability.EpcisInterop;

public sealed class EpcisInteropTests
{
    [Fact]
    public void Internal_transformations_use_epcis_event_and_un_cefact_quantity_codes()
    {
        Guid tenantId = Guid.NewGuid();
        Guid eventId = Guid.NewGuid();
        var traceEvent = new TraceEvent(eventId, Guid.NewGuid(), TraceEventType.Transformation,
            "harvesting", "harvested", new DateTimeOffset(2026, 8, 30, 8, 0, 0, TimeSpan.FromHours(2)),
            "field", Guid.NewGuid(), "HarvestLot", Guid.NewGuid(), null);
        TraceEventObject[] objects =
        [
            new(Guid.NewGuid(), eventId, TraceObjectType.InputStockLot, Guid.NewGuid(), TraceObjectRole.Input, 25m, "kg"),
            new(Guid.NewGuid(), eventId, TraceObjectType.HarvestLot, Guid.NewGuid(), TraceObjectRole.Output, 20m, "L")
        ];

        JsonObject result = EpcisInterop.InternalEvent(tenantId, traceEvent, objects);

        Assert.Equal("TransformationEvent", result["type"]!.GetValue<string>());
        Assert.Equal("commissioning", result["bizStep"]!.GetValue<string>());
        Assert.Equal("active", result["disposition"]!.GetValue<string>());
        Assert.Equal("KGM", result["inputQuantityList"]![0]!["uom"]!.GetValue<string>());
        Assert.Equal("LTR", result["outputQuantityList"]![0]!["uom"]!.GetValue<string>());
    }

    [Fact]
    public void Capture_validation_accepts_epcis_2_document_and_preserves_extensions()
    {
        const string document = """
            {
              "@context": "https://ref.gs1.org/standards/epcis/epcis-context.jsonld",
              "id": "urn:uuid:45d0ea06-4105-4a03-8132-58f17b811236",
              "type": "EPCISDocument",
              "schemaVersion": "2.0",
              "creationDate": "2026-08-30T06:00:00Z",
              "epcisBody": { "eventList": [{
                "eventID": "urn:uuid:37a48010-09bb-4b5f-8918-ad2c82cf65be",
                "type": "ObjectEvent",
                "action": "OBSERVE",
                "bizStep": "shipping",
                "disposition": "in_transit",
                "eventTime": "2026-08-30T08:00:00+02:00",
                "eventTimeZoneOffset": "+02:00",
                "epcList": ["urn:epc:id:sgtin:0614141.107346.2017"],
                "https://example.com/temperature": 4.2
              }]}
            }
            """;

        EpcisCaptureValidation result = EpcisInterop.ValidateCapture(Encoding.UTF8.GetBytes(document));

        Assert.Empty(result.Issues);
        EpcisCapturedEnvelope captured = Assert.Single(result.Events);
        Assert.Equal("shipping", captured.BusinessStep);
        Assert.Contains("https://example.com/temperature", captured.PayloadJson);
        Assert.Equal(64, captured.Sha256.Length);
    }

    [Fact]
    public void Capture_validation_reports_invalid_action_timezone_and_compact_vocabulary()
    {
        const string document = """
            {"@context":{},"type":"EPCISDocument","schemaVersion":"2.0","epcisBody":{"eventList":[{
              "type":"ObjectEvent","eventTime":"2026-08-30T08:00:00Z","eventTimeZoneOffset":"+15:00",
              "action":"UPSERT","bizStep":"not-a-cbv-value","epcList":["not-a-uri"]
            }]}}
            """;

        EpcisCaptureValidation result = EpcisInterop.ValidateCapture(Encoding.UTF8.GetBytes(document));

        Assert.Contains(result.Issues, x => x.Code == "event_action_invalid");
        Assert.Contains(result.Issues, x => x.Code == "event_timezone_invalid");
        Assert.Contains(result.Issues, x => x.Code == "vocabulary_invalid");
        Assert.Contains(result.Issues, x => x.Code == "identifier_invalid");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Epcis_repository_rows_are_append_only_in_the_context(bool mutateExchange)
    {
        Guid tenantId = Guid.NewGuid();
        Guid farmId = Guid.NewGuid();
        Guid exchangeId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var exchange = new EpcisExchange(exchangeId, farmId, EpcisExchangeDirection.Capture,
            "EPCISDocument", $"urn:uuid:{Guid.NewGuid():D}", "2.0", "2.0.0", new string('a', 64),
            1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, null, null, "{}", DateTimeOffset.UtcNow, "test");
        var captured = new EpcisCapturedEvent(Guid.NewGuid(), exchangeId, farmId,
            $"urn:uuid:{Guid.NewGuid():D}", "ObjectEvent", DateTimeOffset.UtcNow, null, "shipping",
            "in_transit", "[]", new string('b', 64), "{}");
        db.EpcisExchanges.Add(exchange);
        db.EpcisCapturedEvents.Add(captured);
        await db.SaveChangesAsync();

        if (mutateExchange) db.Entry(exchange).State = EntityState.Modified;
        else db.Entry(captured).State = EntityState.Modified;

        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public bool IsResolved => true;
        public Guid TenantId { get; } = tenantId;
    }
}
