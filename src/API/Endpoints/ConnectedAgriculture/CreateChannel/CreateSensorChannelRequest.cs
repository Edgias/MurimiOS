using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateChannel;
public sealed record CreateSensorChannelRequest(Guid? Id, string ChannelKey, string MetricCode, string Unit, decimal? MinimumValidValue, decimal? MaximumValidValue, int SamplingIntervalSeconds);
