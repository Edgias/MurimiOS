using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListInvoices;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
using BillingInvoiceStatus = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoiceStatus;

public sealed record ListInvoicesQuery(BillingInvoiceStatus? status) : IRequest<UseCaseResult>;
