using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateBillingAccount;
public sealed record CreateBillingAccountRequest(Guid? Id, string Provider, string ProviderCustomerReference, string BillingEmail, string? TaxIdentifier);
