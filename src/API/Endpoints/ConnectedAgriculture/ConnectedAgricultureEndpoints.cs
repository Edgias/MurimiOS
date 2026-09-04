using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture;
using ChangeBusinessDocumentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeBusinessDocument.ChangeBusinessDocumentEndpoint;
using ChangeDerivedMediaHoldEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeDerivedMediaHold.ChangeDerivedMediaHoldEndpoint;
using ChangeDeviceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeDevice.ChangeDeviceEndpoint;
using ChangeEquipmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeEquipment.ChangeEquipmentEndpoint;
using ChangeTaskFileEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeTaskFile.ChangeTaskFileEndpoint;
using CreateBusinessDocumentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateBusinessDocument.CreateBusinessDocumentEndpoint;
using CreateChannelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateChannel.CreateChannelEndpoint;
using CreateDeviceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateDevice.CreateDeviceEndpoint;
using CreateEquipmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateEquipment.CreateEquipmentEndpoint;
using CreateTaskFileEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateTaskFile.CreateTaskFileEndpoint;
using DisableChannelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.DisableChannel.DisableChannelEndpoint;
using FreshnessEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.Freshness.FreshnessEndpoint;
using IngestReadingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings.IngestReadingsEndpoint;
using IngestTelemetryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestTelemetry.IngestTelemetryEndpoint;
using ListBusinessDocumentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListBusinessDocuments.ListBusinessDocumentsEndpoint;
using ListDerivedMediaEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListDerivedMedia.ListDerivedMediaEndpoint;
using ListEquipmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListEquipment.ListEquipmentEndpoint;
using ListTaskFilesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListTaskFiles.ListTaskFilesEndpoint;
using QueryReadingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.QueryReadings.QueryReadingsEndpoint;
using QueryTelemetryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.QueryTelemetry.QueryTelemetryEndpoint;
using RegisterDerivedMediaEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.RegisterDerivedMedia.RegisterDerivedMediaEndpoint;


public static class ConnectedAgricultureEndpoints
{
    public static void MapConnectedAgricultureEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder connected = api.MapGroup("/connected").WithTags("Connected agriculture");
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListEquipment.ListEquipmentEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateEquipment.CreateEquipmentEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeEquipment.ChangeEquipmentEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListDevices.ListDevicesEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateDevice.CreateDeviceEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeDevice.ChangeDeviceEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateChannel.CreateChannelEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.DisableChannel.DisableChannelEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.QueryReadings.QueryReadingsEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings.IngestReadingsEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.QueryTelemetry.QueryTelemetryEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestTelemetry.IngestTelemetryEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.Freshness.FreshnessEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListTaskFiles.ListTaskFilesEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateTaskFile.CreateTaskFileEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeTaskFile.ChangeTaskFileEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListBusinessDocuments.ListBusinessDocumentsEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateBusinessDocument.CreateBusinessDocumentEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeBusinessDocument.ChangeBusinessDocumentEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListDerivedMedia.ListDerivedMediaEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.RegisterDerivedMedia.RegisterDerivedMediaEndpoint.Map(connected);
        global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeDerivedMediaHold.ChangeDerivedMediaHoldEndpoint.Map(connected);
    }
}
