using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.RetireRotationPolicy;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;

public sealed record RetireRotationPolicyCommand(Guid policyId) : IRequest<UseCaseResult>;
