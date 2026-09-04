using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateBusinessDocument;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using ExchangeDirection = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.ExchangeDirection;
public sealed record CreateBusinessDocumentRequest(Guid? Id, Guid ConnectionId, BusinessDocumentType DocumentType, ExchangeDirection Direction, string ExternalDocumentId, int SchemaVersion, string MappingVersion, JsonElement Payload, string PayloadSha256, DateTimeOffset OccurredAt);
