using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.ConsumeSelfService;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierLinkEvent = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierLinkEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using SupplierSelfServiceTokenStatus = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceTokenStatus;

public sealed record ConsumeSelfServiceCommand(string token) : IRequest<UseCaseResult>;
