using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateTaskFile;
using MachineryTaskDirection = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskDirection;
using MachineryTaskFormat = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFormat;
public sealed record CreateMachineryTaskFileRequest(Guid? Id, Guid EquipmentId, Guid? WorkOrderId, Guid? FieldId, MachineryTaskDirection Direction, MachineryTaskFormat Format, string FileName, string ObjectKey, long ByteSize, string Sha256, int SchemaVersion, string MappingVersion, string? ExternalTaskId);
