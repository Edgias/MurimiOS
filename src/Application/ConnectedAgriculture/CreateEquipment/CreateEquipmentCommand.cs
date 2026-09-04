using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateEquipment;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentType = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CreateEquipmentCommand(Guid farmId, CreateEquipmentRequest request) : IRequest<UseCaseResult>;
