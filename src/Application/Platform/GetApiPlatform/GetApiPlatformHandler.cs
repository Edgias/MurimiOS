using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Platform.GetApiPlatform;


public sealed partial class GetApiPlatformHandler() : IRequestHandler<GetApiPlatformQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiPlatformQuery message, CancellationToken cancellationToken) => Task.FromResult(ExecuteAsync());
private static UseCaseResult ExecuteAsync() =>UseCaseResults.Ok(new PlatformInfoResponse
        {
            name = "MurimiOS",
            apiVersion = "v1",
            syncSchemaVersions = new[] { 1 },
            syncCommands = new[]
            {
                "RecordFieldOperation",
                "RecordCropZoneOperation",
                "ApplyInputToCropZone",
                "RecordHarvestLot",
                "RecordLivestockMeasurement",
                "RecordLivestockTreatment",
                "RecordLivestockMovement",
                "RecordLivestockHealthEvent",
                "AllocateLivestockFeed",
                "StartWorkOrder",
                "CompleteWorkOrder"
            }
        });

}
