using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalRequest;
public sealed record CreateDelegatedApprovalRequest(Guid? Id, Guid PolicyId, Guid? FarmId, string TargetType, Guid TargetId, string ActionType, string RequestJson, DateTimeOffset ExpiresAt);
