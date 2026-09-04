using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeBillingAccount;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;

public sealed record ChangeBillingAccountCommand(Guid accountId, string command) : IRequest<UseCaseResult>;
