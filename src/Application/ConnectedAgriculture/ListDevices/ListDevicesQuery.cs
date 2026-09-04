using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDevices;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed record ListDevicesQuery(Guid farmId, ConnectedDeviceStatus? status) : IRequest<UseCaseResult>;
