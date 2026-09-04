using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Sync.GetSyncPull;


public sealed record SyncChangeResponse(
    long Sequence,
    string EntityType,
    Guid EntityId,
    string Operation,
    JsonElement Payload,
    DateTimeOffset ChangedAt);
