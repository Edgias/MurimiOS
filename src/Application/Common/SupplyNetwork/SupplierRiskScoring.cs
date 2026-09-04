using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;

namespace EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;

using SupplierRiskBand = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskBand;

public static class SupplierRiskScoring
{
    public const string ModelVersion = "supplier-risk-v1";

    public static SupplierRiskResult Calculate(SupplierRiskInputs inputs)
    {
        int evidenceTotal = inputs.EvidenceRequested + inputs.EvidenceSubmitted + inputs.EvidenceAccepted + inputs.EvidenceRejected;
        decimal evidenceRisk = evidenceTotal == 0 ? 50m : decimal.Clamp(
            (inputs.EvidenceRequested * 45m + inputs.EvidenceSubmitted * 20m + inputs.EvidenceRejected * 75m
             + inputs.EvidenceOverdue * 35m) / evidenceTotal, 0m, 100m);
        decimal deliveryRisk = inputs.PastDueAllocatedQuantity <= 0 ? 0m : decimal.Clamp(
            100m * (1m - inputs.PastDueFulfilledQuantity / inputs.PastDueAllocatedQuantity), 0m, 100m);
        int contractTotal = inputs.ActiveContracts + inputs.CompletedContracts + inputs.TerminatedContracts;
        decimal contractRisk = contractTotal == 0 ? 40m : decimal.Clamp(
            100m * inputs.TerminatedContracts / contractTotal + 15m * inputs.ActiveContracts / contractTotal, 0m, 100m);
        decimal accountRisk = decimal.Clamp(inputs.AccountRiskScore, 0, 100);
        int score = (int)decimal.Round(evidenceRisk * .35m + deliveryRisk * .35m
            + contractRisk * .20m + accountRisk * .10m, 0, MidpointRounding.AwayFromZero);
        SupplierRiskBand band = score switch
        {
            <= 24 => SupplierRiskBand.Low,
            <= 49 => SupplierRiskBand.Moderate,
            <= 74 => SupplierRiskBand.High,
            _ => SupplierRiskBand.Critical
        };
        string components = JsonSerializer.Serialize(new
        {
            evidence = decimal.Round(evidenceRisk, 2),
            delivery = decimal.Round(deliveryRisk, 2),
            contract = decimal.Round(contractRisk, 2),
            account = decimal.Round(accountRisk, 2),
            weights = new { evidence = .35m, delivery = .35m, contract = .20m, account = .10m }
        });
        return new(score, band, components, JsonSerializer.Serialize(inputs));
    }
}
