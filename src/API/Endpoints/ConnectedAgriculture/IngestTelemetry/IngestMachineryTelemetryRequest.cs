using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestTelemetry;
public sealed record IngestMachineryTelemetryRequest(Guid? Id, Guid EquipmentId, Guid? DeviceId, decimal? EngineHours, decimal? SpeedKph, decimal? FuelLevelPercent, string State, double? Latitude, double? Longitude);
