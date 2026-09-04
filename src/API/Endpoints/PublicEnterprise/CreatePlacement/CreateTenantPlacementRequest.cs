using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantPlacement;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreatePlacement;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
public sealed record CreateTenantPlacementRequest(Guid? Id, string Region, string DatabaseKey, TenantDatabaseMode Mode, string ResidencyCountry, bool IsPrimary, DateTimeOffset? EffectiveFrom);
