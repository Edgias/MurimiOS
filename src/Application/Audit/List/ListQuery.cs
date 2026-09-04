using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Audit.List;

public sealed record ListQuery(DateTimeOffset? before, string? entityType, Guid? entityId, int? limit) : IRequest<UseCaseResult>;
