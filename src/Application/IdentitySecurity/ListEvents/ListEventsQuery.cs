using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListEvents;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;

public sealed record ListEventsQuery(string? eventType, SecurityEventOutcome? outcome, DateTimeOffset? from, DateTimeOffset? to, int? limit) : IRequest<UseCaseResult>;
