using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyContract;

namespace EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

public static class ContractPricing
{
    public static ContractPriceQuote Resolve(SupplyContract contract, IEnumerable<ContractPriceRule> rules,
        decimal quantity, string? qualityGrade, DateOnly effectiveOn)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        ContractPriceRule? selected = rules.Where(x => x.ContractId == contract.Id && x.Matches(quantity, qualityGrade, effectiveOn))
            .OrderBy(x => x.Method == ContractPricingMethod.QualityGrade ? 0 : x.Method == ContractPricingMethod.VolumeTier ? 1 : 2)
            .ThenBy(x => x.Priority)
            .ThenByDescending(x => x.MinimumQuantity)
            .FirstOrDefault();
        decimal? unitPrice = selected?.UnitPrice ?? contract.UnitPrice;
        string? currency = selected?.Currency ?? contract.Currency;
        if (!unitPrice.HasValue || string.IsNullOrWhiteSpace(currency))
            throw new InvalidOperationException("No effective contract price applies to this quantity and quality grade.");
        return new(selected?.Id, selected?.Method ?? ContractPricingMethod.Fixed, quantity,
            string.IsNullOrWhiteSpace(qualityGrade) ? null : qualityGrade.Trim(), unitPrice.Value, currency,
            decimal.Round(quantity * unitPrice.Value, 2, MidpointRounding.AwayFromZero), effectiveOn);
    }
}
