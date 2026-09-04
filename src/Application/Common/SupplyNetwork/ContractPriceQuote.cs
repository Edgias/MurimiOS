using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyContract;

namespace EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;

public sealed record ContractPriceQuote(Guid? PriceRuleId, ContractPricingMethod Method, decimal Quantity,
    string? QualityGrade, decimal UnitPrice, string Currency, decimal GrossAmount, DateOnly EffectiveOn);
