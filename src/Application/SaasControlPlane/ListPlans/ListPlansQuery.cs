using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListPlans;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed record ListPlansQuery(SaasPlanStatus? status) : IRequest<UseCaseResult>;
