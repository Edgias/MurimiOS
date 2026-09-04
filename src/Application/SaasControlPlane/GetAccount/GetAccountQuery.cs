using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.GetAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed record GetAccountQuery() : IRequest<UseCaseResult>;
