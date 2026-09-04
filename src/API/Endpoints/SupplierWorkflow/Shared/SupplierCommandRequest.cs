using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.Shared;
public sealed record SupplierCommandRequest(string? Notes, Guid? ClaimedTenantId = null);
