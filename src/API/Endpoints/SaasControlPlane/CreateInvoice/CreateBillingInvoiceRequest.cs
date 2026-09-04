using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateInvoice;
public sealed record CreateBillingInvoiceRequest(Guid? Id, Guid BillingAccountId, Guid? SubscriptionId, string InvoiceNumber, string? ProviderInvoiceReference, decimal Subtotal, decimal Tax, decimal Total, string Currency, DateTimeOffset IssuedAt, DateTimeOffset DueAt);
