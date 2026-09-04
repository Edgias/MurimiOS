using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListDevices;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentityDeviceStatus = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDeviceStatus;

public sealed record ListDevicesQuery(string? subject, IdentityDeviceStatus? status) : IRequest<UseCaseResult>;
