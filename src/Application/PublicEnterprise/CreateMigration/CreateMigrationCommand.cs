using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateMigration;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantMigrationKind = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationKind;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;

public sealed record CreateMigrationCommand(CreateTenantMigrationRequest request) : IRequest<UseCaseResult>;
