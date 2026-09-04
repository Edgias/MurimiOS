using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListBusinessDocuments;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;

using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;

public sealed record ListBusinessDocumentsQuery(Guid farmId, ExchangeStatus? status, BusinessDocumentType? type) : IRequest<UseCaseResult>;
