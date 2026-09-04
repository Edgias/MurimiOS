using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record RequestQuotaDecision(bool Allowed, int RequestRemaining, int DailyRemaining, int RetryAfterSeconds);
