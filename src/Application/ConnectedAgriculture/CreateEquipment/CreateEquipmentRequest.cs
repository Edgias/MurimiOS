using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateEquipment;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentType = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CreateEquipmentRequest(Guid? Id, string Code, string Name, EquipmentType Type, string? Manufacturer, string? Model, string? SerialNumber, int? ModelYear);
