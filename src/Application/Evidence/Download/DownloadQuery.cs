using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.Download;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

public sealed record DownloadQuery(Guid evidenceId) : IRequest<UseCaseResult>;
