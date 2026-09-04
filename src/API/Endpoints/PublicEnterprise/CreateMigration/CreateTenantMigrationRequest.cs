using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantPlacement;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateMigration;
using TenantMigrationKind = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationKind;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
public sealed record CreateTenantMigrationRequest(Guid? Id, TenantMigrationKind Kind, Guid SourcePlacementId, string DestinationRegion, string DestinationDatabaseKey, TenantDatabaseMode DestinationMode);
