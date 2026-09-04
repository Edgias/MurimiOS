using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.StorageLot;

public static class TraceabilityMassBalance
{
    public static void EnsureBalanced(decimal inputs, decimal outputs, decimal loss)
    {
        if (inputs <= 0 || outputs <= 0 || loss < 0) throw new ArgumentOutOfRangeException(nameof(inputs), "Inputs and outputs must be positive and loss cannot be negative.");
        if (Math.Abs(inputs - outputs - loss) > 0.0001m)
            throw new InvalidOperationException($"Mass balance failed: inputs {inputs}, outputs {outputs}, declared loss {loss}.");
    }

    public static decimal Available(decimal original, decimal consumed) => Math.Max(0, original - consumed);
}
