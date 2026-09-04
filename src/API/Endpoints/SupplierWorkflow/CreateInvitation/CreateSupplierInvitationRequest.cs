using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateInvitation;

public sealed record CreateSupplierInvitationRequest(Guid? Id, string SupplierName, string Email, DateTimeOffset ExpiresAt);
