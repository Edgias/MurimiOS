using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.Get;

public sealed record GetQuery(bool? unreadOnly, int? limit) : IRequest<UseCaseResult>;
