using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateTaskFile;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;
using MachineryTaskDirection = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskDirection;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;
using MachineryTaskFormat = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFormat;

public sealed class CreateTaskFileHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateTaskFileCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateTaskFileCommand message, CancellationToken cancellationToken) => CreateTaskFileAsync(message.farmId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateTaskFileAsync(Guid farmId, CreateMachineryTaskFileRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await FarmAllowed(farmId, db, scope, ct)) return UseCaseResults.NotFound();
        if (!await db.EquipmentAssets.AnyAsync(x => x.Id == request.EquipmentId && x.FarmId == farmId && x.Status != EquipmentStatus.Retired, ct)) return Bad("equipment_scope_invalid", "The equipment is unavailable for this farm.");
        if (request.WorkOrderId.HasValue && !await db.WorkOrders.AnyAsync(x => x.Id == request.WorkOrderId && x.FarmId == farmId, ct)) return Bad("work_order_scope_invalid", "The work order does not belong to this farm.");
        if (request.FieldId.HasValue && !await db.FarmFields.AnyAsync(x => x.Id == request.FieldId && x.FarmId == farmId, ct)) return Bad("field_scope_invalid", "The field does not belong to this farm.");
        try { var file = new MachineryTaskFile(Id(request.Id), farmId, request.EquipmentId, request.WorkOrderId, request.FieldId, request.Direction, request.Format, request.FileName, request.ObjectKey, request.ByteSize, request.Sha256, request.SchemaVersion, request.MappingVersion, request.ExternalTaskId, DateTimeOffset.UtcNow); db.MachineryTaskFiles.Add(file); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/connected/task-files/{file.Id}", file); }
        catch (ArgumentException e) { return Bad("task_file_invalid", e.Message); }
    }


    private static async Task<bool> FarmAllowed(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct) => await scope.CanAccessFarmAsync(farmId, ct) && await db.Farms.AnyAsync(x => x.Id == farmId, ct);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
