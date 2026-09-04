using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.GetPreferences;

public sealed record GetPreferencesQuery() : IRequest<UseCaseResult>;
