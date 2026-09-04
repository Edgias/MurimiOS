using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRatePolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed record CreateRatePolicyCommand(CreateApiRateLimitPolicyRequest request) : IRequest<UseCaseResult>;
