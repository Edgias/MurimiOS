using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings;
public sealed record IngestSensorReadingItem(Guid? Id, Guid ChannelId, decimal Value, string Unit, DateTimeOffset ObservedAt, string ExternalReadingId, double? Latitude, double? Longitude);
