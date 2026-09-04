using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.AcceptInvitation;
public sealed record AcceptSupplierInvitationRequest(string Token, string SupplierCode, string ContactName, string? Phone);
