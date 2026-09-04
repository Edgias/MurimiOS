using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.GetSubmission;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;

public sealed record GetSubmissionQuery(Guid submissionId) : IRequest<UseCaseResult>;
