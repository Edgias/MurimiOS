using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ProvisionAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed record ProvisionTenantAccountRequest(Guid? Id, string LegalName, string DisplayName, string Slug, string CountryCode, string BillingCurrency, string TimeZone);
