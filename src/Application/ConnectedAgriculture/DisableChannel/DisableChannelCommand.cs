using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.DisableChannel;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;

using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed record DisableChannelCommand(Guid channelId) : IRequest<UseCaseResult>;
