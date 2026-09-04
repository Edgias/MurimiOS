using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplyContract;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class ContractPriceRule : TenantEntity
{
    private ContractPriceRule() { }

    public ContractPriceRule(Guid id, Guid contractId, int priority, ContractPricingMethod method,
        decimal minimumQuantity, decimal? maximumQuantity, string? qualityGrade, decimal unitPrice,
        string currency, DateOnly effectiveFrom, DateOnly effectiveTo) : base(id)
    {
        if (contractId == Guid.Empty || priority < 1 || !Enum.IsDefined(method) || minimumQuantity < 0
            || maximumQuantity <= minimumQuantity || unitPrice < 0 || effectiveTo < effectiveFrom)
            throw new ArgumentException("The price-rule range, priority, price or effective period is invalid.");
        if (method == ContractPricingMethod.QualityGrade && string.IsNullOrWhiteSpace(qualityGrade)
            || method != ContractPricingMethod.QualityGrade && !string.IsNullOrWhiteSpace(qualityGrade))
            throw new ArgumentException("Only quality-grade rules may specify a quality grade.");
        ContractId = contractId;
        Priority = priority;
        Method = method;
        MinimumQuantity = minimumQuantity;
        MaximumQuantity = maximumQuantity;
        QualityGrade = SupplierInvitation.Optional(qualityGrade, 80);
        UnitPrice = unitPrice;
        Currency = CurrencyCode(currency);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public Guid ContractId { get; private set; }
    public int Priority { get; private set; }
    public ContractPricingMethod Method { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal? MaximumQuantity { get; private set; }
    public string? QualityGrade { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = "";
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly EffectiveTo { get; private set; }

    public bool Matches(decimal quantity, string? qualityGrade, DateOnly on) =>
        quantity >= MinimumQuantity && (!MaximumQuantity.HasValue || quantity < MaximumQuantity.Value)
        && on >= EffectiveFrom && on <= EffectiveTo
        && (Method != ContractPricingMethod.QualityGrade
            || string.Equals(QualityGrade, qualityGrade?.Trim(), StringComparison.OrdinalIgnoreCase));

    internal static string CurrencyCode(string? value)
    {
        string result = value?.Trim().ToUpperInvariant() ?? "";
        if (result.Length != 3 || result.Any(x => !char.IsAsciiLetterUpper(x))) throw new ArgumentException("Currency must be a three-letter ISO code.");
        return result;
    }
}
