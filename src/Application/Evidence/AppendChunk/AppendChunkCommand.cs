using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.AppendChunk;
using MediaUploadStatus = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadStatus;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;

public sealed record AppendChunkCommand(Guid uploadId, IncomingContent request) : IRequest<UseCaseResult>;
