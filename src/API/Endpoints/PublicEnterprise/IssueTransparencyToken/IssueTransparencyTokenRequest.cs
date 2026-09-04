using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.IssueTransparencyToken;
public sealed record IssueTransparencyTokenRequest(Guid? Id, Guid HarvestLotId, DateTimeOffset? ExpiresAt);
