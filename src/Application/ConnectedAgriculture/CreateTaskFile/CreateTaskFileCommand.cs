using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
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

public sealed record CreateTaskFileCommand(Guid farmId, CreateMachineryTaskFileRequest request) : IRequest<UseCaseResult>;
