using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateMedicine;

public sealed record CreateVeterinaryMedicineRequest(
    string Name,
    string ActiveIngredient,
    string Unit,
    int MeatWithdrawalDays,
    int MilkWithdrawalHours);
