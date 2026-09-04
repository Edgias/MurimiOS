using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings;
public sealed record IngestSensorReadingsRequest(Guid DeviceId, string? FirmwareVersion, IReadOnlyList<IngestSensorReadingItem>? Readings);
