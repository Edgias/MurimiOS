using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangePlan;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed record ChangePlanCommand(Guid planId, string command) : IRequest<UseCaseResult>;
