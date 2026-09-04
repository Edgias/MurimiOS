using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.GetApiCropZonesCropZoneIdGuidEvidence;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

public sealed record GetApiCropZonesCropZoneIdGuidEvidenceQuery(Guid cropZoneId) : IRequest<UseCaseResult>;
