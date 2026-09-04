using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeBusinessDocument;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;

public sealed record ChangeBusinessDocumentCommand(Guid documentId, string command, ExchangeCommandRequest request) : IRequest<UseCaseResult>;
