using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateDevice;
using ConnectedDeviceKind = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceKind;
using ConnectedDeviceProtocol = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceProtocol;
public sealed record CreateConnectedDeviceRequest(Guid? Id, string DeviceKey, string Name, ConnectedDeviceKind Kind, ConnectedDeviceProtocol Protocol, Guid? ConnectionId, Guid? EquipmentId, Guid? FieldId, Guid? HoldingId, string? FirmwareVersion);
