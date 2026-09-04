using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiCropZonesCropZoneIdGuidOperations;


public sealed record FieldOperationResponse(
    Guid Id,
    Guid CropZoneId,
    string OperationType,
    DateTimeOffset OccurredAt,
    DateTimeOffset RecordedAt,
    string? Notes);
