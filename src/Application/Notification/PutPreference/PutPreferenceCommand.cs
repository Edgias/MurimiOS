using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.PutPreference;

public sealed record PutPreferenceCommand(string category, UpdateNotificationPreferenceRequest request) : IRequest<UseCaseResult>;
