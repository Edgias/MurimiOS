using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListEquipment;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;

public sealed record ListEquipmentQuery(Guid farmId, EquipmentStatus? status) : IRequest<UseCaseResult>;
