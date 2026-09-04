using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.List;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;

public sealed record ListQuery(string? entityType, Guid? entityId, int? limit) : IRequest<UseCaseResult>;
