using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.InitiateUpload;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using MediaUploadStatus = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadStatus;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;

public sealed record InitiateUploadCommand(InitiateEvidenceUploadRequest request) : IRequest<UseCaseResult>;
