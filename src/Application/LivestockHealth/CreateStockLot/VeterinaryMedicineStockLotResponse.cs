using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateStockLot;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;

public sealed record VeterinaryMedicineStockLotResponse(
    Guid Id,
    Guid MedicineId,
    string BatchNumber,
    DateOnly ReceivedOn,
    DateOnly? ExpiresOn,
    decimal InitialQuantity,
    decimal QuantityOnHand,
    string Unit,
    decimal EnteredInitialQuantity,
    string EnteredUnit,
    string DisplayUnit);
