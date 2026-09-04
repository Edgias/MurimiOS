using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateClient;

public sealed record CreatePublicApiClientRequest(Guid? Id, string Name, IReadOnlyList<string> Scopes, IReadOnlyList<Guid> FarmIds, DateTimeOffset? ExpiresAt);
