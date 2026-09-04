using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateProfile;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;

public sealed record CreateLivestockRegulatoryProfileRequest(Guid? Id, string ProfileKey, int Version, string Name,
    LivestockRegulatoryExchangeType ExchangeType, string Jurisdiction, string Species, string ProviderCode,
    string SchemaVersion, string MediaType, int? ReportingDeadlineHours, int? RegisterDeadlineHours,
    DateOnly EffectiveFrom, DateOnly? EffectiveTo, string SourceUri);
