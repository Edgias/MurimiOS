using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListTaskFiles;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;

public sealed record ListTaskFilesQuery(Guid farmId, ExchangeStatus? status) : IRequest<UseCaseResult>;
