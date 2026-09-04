using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.ListSubmissions;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockExchangeSubmissionStatus = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmissionStatus;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;

public sealed record ListSubmissionsQuery(LivestockExchangeSubmissionStatus? status, LivestockRegulatoryExchangeType? exchangeType, Guid? farmId) : IRequest<UseCaseResult>;
