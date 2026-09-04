using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.CreatePriceRule;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;
public sealed record CreateContractPriceRuleRequest(Guid? Id, int Priority, ContractPricingMethod Method,
    decimal MinimumQuantity, decimal? MaximumQuantity, string? QualityGrade, decimal UnitPrice,
    string Currency, DateOnly EffectiveFrom, DateOnly EffectiveTo);
