using System.Net;
using System.Text;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.LabourCostEntry;
using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
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
using EdgIQ.MurimiOS.Infrastructure.Integrations;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using CanonicalMachineryRestAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.CanonicalMachineryRestAdapter;
using CanonicalErpRestAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.CanonicalErpRestAdapter;
using CanonicalAccountingRestAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.CanonicalAccountingRestAdapter;
using MqttPayloadDecoder = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttPayloadDecoder;
using AccountingBatchValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.AccountingBatchValue;
using BusinessDocumentValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.BusinessDocumentValue;
using MachineryTelemetryValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MachineryTelemetryValue;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using SensorReadingValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorReadingValue;

public sealed class ConnectedProviderAdapterTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 31, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ThingsStack_uplink_maps_device_frame_channels_and_location()
    {
        string payload = """
            {"end_device_ids":{"device_id":"soil-01","dev_eui":"ABC"},"received_at":"2026-08-31T11:59:00Z",
             "uplink_message":{"f_cnt":42,"decoded_payload":{"soil_moisture":31.5,"battery":92},
             "locations":{"frm-payload":{"latitude":-17.82,"longitude":31.05}},
             "version_ids":{"firmware_version":"2.4.1"}}}
            """;

        ProviderRecord result = MqttPayloadDecoder.DecodeSensor("v3/app/devices/soil-01/up", payload,
            "{\"units\":{\"soil_moisture\":\"%\",\"battery\":\"%\"}}", Now, true);

        SensorReadingValue readings = Assert.IsType<SensorReadingValue>(result.Normalized);
        Assert.Equal("soil-01", readings.DeviceKey);
        Assert.Equal("soil-01:42", result.ExternalRecordId);
        Assert.Equal("2.4.1", readings.FirmwareVersion);
        Assert.Equal(2, readings.Channels.Count);
        Assert.Equal(-17.82, readings.Channels[0].Latitude);
    }

    [Fact]
    public void Generic_mqtt_payload_maps_configured_measurement_object()
    {
        ProviderRecord result = MqttPayloadDecoder.DecodeSensor("farm/device", """
            {"sensor":"weather-1","time":"2026-08-31T10:00:00Z","values":{"temp":25.4,"rain":2.1}}
            """, """
            {"deviceKeyProperty":"sensor","observedAtProperty":"time","measurementsProperty":"values",
             "units":{"temp":"C","rain":"mm"}}
            """, Now, false);

        SensorReadingValue readings = Assert.IsType<SensorReadingValue>(result.Normalized);
        Assert.Equal("weather-1", readings.DeviceKey);
        Assert.Contains(readings.Channels, x => x.ChannelKey == "temp" && x.Value == 25.4m && x.Unit == "C");
    }

    [Fact]
    public void Mqtt_machinery_payload_maps_telematics()
    {
        ProviderRecord result = MqttPayloadDecoder.DecodeMachinery("machines/tractor-1", """
            {"id":"msg-7","equipmentCode":"TR-01","deviceKey":"gw-1","observedAt":"2026-08-31T10:00:00Z",
             "engineHours":123.5,"speedKph":8.2,"fuelLevelPercent":74,"state":"working","latitude":-17.8,"longitude":31.0}
            """, "{}", Now);

        MachineryTelemetryValue value = Assert.IsType<MachineryTelemetryValue>(result.Normalized);
        Assert.Equal("TR-01", value.EquipmentCode);
        Assert.Equal(123.5m, value.EngineHours);
        Assert.Equal("working", value.State);
    }

    [Fact]
    public async Task Canonical_machinery_rest_uses_cursor_and_bearer_token()
    {
        var handler = new CaptureHandler("""
            {"records":[{"id":"tm-1","observedAt":"2026-08-31T10:00:00Z","equipmentCode":"TR-01",
             "engineHours":10,"speedKph":4,"fuelLevelPercent":80,"state":"idle"}],"nextCursor":"next-2"}
            """);
        var adapter = new CanonicalMachineryRestAdapter(new HttpClient(handler));

        ProviderIngestionResult result = await adapter.IngestAsync(Context(IntegrationFeedType.Machinery,
            "{\"endpoint\":\"https://provider.example/telemetry\"}", "{\"cursorParameter\":\"after\"}",
            "{\"cursor\":\"old-1\"}", "{\"scheme\":\"Bearer\",\"token\":\"secret\"}"), default);

        Assert.Single(result.Records);
        Assert.Contains("after=old-1", handler.Uri!.Query);
        Assert.Equal("Bearer", handler.AuthorizationScheme);
        Assert.Contains("next-2", result.CheckpointJson);
    }

    [Fact]
    public async Task Canonical_erp_and_accounting_responses_map_to_domain_contracts()
    {
        var erp = new CanonicalErpRestAdapter(new HttpClient(new CaptureHandler("""
            {"documents":[{"id":"po-7","type":"PurchaseOrder","occurredAt":"2026-08-31T09:00:00Z",
             "schemaVersion":2,"mappingVersion":"erp-v2","payload":{"supplier":"SUP-1","total":42}}]}
            """)));
        ProviderIngestionResult erpResult = await erp.IngestAsync(Context(IntegrationFeedType.Erp,
            "{\"endpoint\":\"https://erp.example/documents\"}", "{}", null, null), default);
        BusinessDocumentValue document = Assert.IsType<BusinessDocumentValue>(Assert.Single(erpResult.Records).Normalized);
        Assert.Equal(BusinessDocumentType.PurchaseOrder, document.DocumentType);
        Assert.Equal(2, document.SchemaVersion);

        var accounting = new CanonicalAccountingRestAdapter(new HttpClient(new CaptureHandler("""
            {"batches":[{"id":"bank-8","statementStart":"2026-08-01","statementEnd":"2026-08-31","currency":"USD",
             "lines":[{"id":"line-1","occurredAt":"2026-08-15T12:00:00Z","entryType":"Expense","amount":15.50,
             "accountCode":"FUEL","reference":"R1","description":"Diesel"}]}]}
            """)));
        ProviderIngestionResult accountingResult = await accounting.IngestAsync(Context(IntegrationFeedType.Accounting,
            "{\"endpoint\":\"https://accounts.example/statements\"}", "{}", null, null), default);
        AccountingBatchValue batch = Assert.IsType<AccountingBatchValue>(Assert.Single(accountingResult.Records).Normalized);
        Assert.Equal("bank-8", batch.ExternalBatchId);
        Assert.Equal(FinanceEntryType.Expense, Assert.Single(batch.Lines).EntryType);
    }

    private static ProviderIngestionContext Context(IntegrationFeedType type, string connection, string feed,
        string? checkpoint, string? credential) => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
        type, IntegrationTargetType.Farm, null, "source", connection, feed, checkpoint, credential,
        null, null, Now, null);

    private sealed class CaptureHandler(string body) : HttpMessageHandler
    {
        public Uri? Uri { get; private set; }
        public string? AuthorizationScheme { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Uri = request.RequestUri;
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        }
    }
}
