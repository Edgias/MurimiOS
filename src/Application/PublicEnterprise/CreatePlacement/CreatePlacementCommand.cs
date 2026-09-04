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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreatePlacement;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;

public sealed record CreatePlacementCommand(CreateTenantPlacementRequest request) : IRequest<UseCaseResult>;
