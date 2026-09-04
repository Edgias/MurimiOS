using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.SupplyNetwork;

public sealed record SupplierRiskInputs(
    int EvidenceRequested,
    int EvidenceSubmitted,
    int EvidenceAccepted,
    int EvidenceRejected,
    int EvidenceOverdue,
    decimal PastDueAllocatedQuantity,
    decimal PastDueFulfilledQuantity,
    int ActiveContracts,
    int CompletedContracts,
    int TerminatedContracts,
    int AccountRiskScore);
