using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRotationPolicy;
public sealed record CreateCredentialRotationPolicyRequest(Guid? Id, Guid? PublicApiClientId, int MaximumAgeDays, int WarningDays, bool RequireExpiry);
