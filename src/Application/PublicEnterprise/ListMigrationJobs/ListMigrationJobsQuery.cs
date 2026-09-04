using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ListMigrationJobs;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;

public sealed record ListMigrationJobsQuery(TenantMigrationStatus? status) : IRequest<UseCaseResult>;
