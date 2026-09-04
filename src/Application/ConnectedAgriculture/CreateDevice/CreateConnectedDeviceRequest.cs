using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateDevice;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;

using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceKind = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceKind;
using ConnectedDeviceProtocol = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceProtocol;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
public sealed record CreateConnectedDeviceRequest(Guid? Id, string DeviceKey, string Name, ConnectedDeviceKind Kind, ConnectedDeviceProtocol Protocol, Guid? ConnectionId, Guid? EquipmentId, Guid? FieldId, Guid? HoldingId, string? FirmwareVersion);
