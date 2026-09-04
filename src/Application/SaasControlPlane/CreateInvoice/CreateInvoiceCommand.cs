using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreateInvoice;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;

using BillingAccountStatus = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccountStatus;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;

public sealed record CreateInvoiceCommand(CreateBillingInvoiceRequest request) : IRequest<UseCaseResult>;
