using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateDisclosurePolicy;
public sealed record CreateDisclosurePolicyRequest(Guid? Id, string Name, string PublicBrandName, IReadOnlyList<string> Attributes, IReadOnlyList<Guid> FarmIds, Guid? DataGrantId, Guid? PreviousPolicyId, string Locale = "en");
