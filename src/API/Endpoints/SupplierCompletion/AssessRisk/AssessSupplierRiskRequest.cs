using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.AssessRisk;

public sealed record AssessSupplierRiskRequest(Guid? RelationshipId, Guid? HostedSupplierId);
