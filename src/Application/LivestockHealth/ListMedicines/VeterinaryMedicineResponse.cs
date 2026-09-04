using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.ListMedicines;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;

public sealed record VeterinaryMedicineResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    string ActiveIngredient,
    string Unit,
    string DisplayUnit,
    int MeatWithdrawalDays,
    int MilkWithdrawalHours);
