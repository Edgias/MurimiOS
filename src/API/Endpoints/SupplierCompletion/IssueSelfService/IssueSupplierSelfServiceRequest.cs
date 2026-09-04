using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.IssueSelfService;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;
public sealed record IssueSupplierSelfServiceRequest(SupplierSelfServiceAction Action, Guid ClaimedTenantId, DateTimeOffset ExpiresAt);
