using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetRetailAccessLog;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

using TransparencyAccessLog = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyAccessLog;

public sealed record GetRetailAccessLogQuery(Guid? tokenId, int? limit) : IRequest<UseCaseResult>;
