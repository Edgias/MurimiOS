using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.VerifyRecovery;
public sealed record VerifyRecoveryRequest(Guid TenantId, Guid RequestId, string Token);
