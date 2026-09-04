using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.FailMigration;
public sealed record FailTenantMigrationRequest(string Code, string Message);
