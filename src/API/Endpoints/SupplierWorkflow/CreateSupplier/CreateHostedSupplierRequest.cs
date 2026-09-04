using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateSupplier;
public sealed record CreateHostedSupplierRequest(Guid? Id, string SupplierCode, string Name, string ContactName, string Email, string? Phone);
