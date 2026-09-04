using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRecovery;
public sealed record CreateRecoveryRequest(Guid? Id, Guid MembershipId, DateTimeOffset? ExpiresAt, int MaximumAttempts = 5);
