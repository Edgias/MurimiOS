using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.FinanceTransaction;

public sealed class CostAllocation : TenantEntity
{
    private CostAllocation() { }

    public CostAllocation(Guid id, Guid transactionId, Guid costCentreId, decimal amount,
        decimal percentage, CostAllocationMethod method, string? notes) : base(id)
    {
        if (transactionId == Guid.Empty || costCentreId == Guid.Empty)
            throw new ArgumentException("A transaction and cost centre are required.");
        if (amount <= 0 || percentage <= 0 || percentage > 100)
            throw new ArgumentException("Allocation amount and percentage must be positive and percentage cannot exceed 100.");
        if (!Enum.IsDefined(method)) throw new ArgumentOutOfRangeException(nameof(method));
        TransactionId = transactionId;
        CostCentreId = costCentreId;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Percentage = decimal.Round(percentage, 6, MidpointRounding.AwayFromZero);
        Method = method;
        Notes = Optional(notes, 1_000);
    }

    public Guid TransactionId { get; private set; }
    public Guid CostCentreId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Percentage { get; private set; }
    public CostAllocationMethod Method { get; private set; }
    public string? Notes { get; private set; }

    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
