using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreateInvoice;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;

using BillingAccountStatus = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccountStatus;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
public sealed record CreateBillingInvoiceRequest(Guid? Id, Guid BillingAccountId, Guid? SubscriptionId, string InvoiceNumber, string? ProviderInvoiceReference, decimal Subtotal, decimal Tax, decimal Total, string Currency, DateTimeOffset IssuedAt, DateTimeOffset DueAt);
