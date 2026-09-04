using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.GetJob;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;

public sealed record GetJobQuery(Guid jobId) : IRequest<UseCaseResult>;
