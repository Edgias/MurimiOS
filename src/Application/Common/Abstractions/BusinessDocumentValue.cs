using System.Text.Json;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;

using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;

public sealed record BusinessDocumentValue(BusinessDocumentType DocumentType, string ExternalDocumentId,
    int SchemaVersion, string MappingVersion, string DocumentJson) : ProviderNormalizedRecord;
