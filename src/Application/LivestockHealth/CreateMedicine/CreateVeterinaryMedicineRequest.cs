using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateMedicine;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;

public sealed record CreateVeterinaryMedicineRequest(
    string Name,
    string ActiveIngredient,
    string Unit,
    int MeatWithdrawalDays,
    int MilkWithdrawalHours);
