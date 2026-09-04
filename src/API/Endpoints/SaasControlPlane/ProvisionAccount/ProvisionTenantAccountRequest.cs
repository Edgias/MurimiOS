using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ProvisionAccount;

public sealed record ProvisionTenantAccountRequest(Guid? Id, string LegalName, string DisplayName, string Slug, string CountryCode, string BillingCurrency, string TimeZone);
