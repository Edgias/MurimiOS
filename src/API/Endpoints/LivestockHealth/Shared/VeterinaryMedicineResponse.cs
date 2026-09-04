using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.Shared;

public sealed record VeterinaryMedicineResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    string ActiveIngredient,
    string Unit,
    string DisplayUnit,
    int MeatWithdrawalDays,
    int MilkWithdrawalHours);
