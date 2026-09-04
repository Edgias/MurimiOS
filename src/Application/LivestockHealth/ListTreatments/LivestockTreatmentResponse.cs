using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.ListTreatments;


public sealed record LivestockTreatmentResponse(
    Guid Id,
    Guid SubjectId,
    Guid MedicineStockLotId,
    string Medicine,
    string BatchNumber,
    decimal Dose,
    string Unit,
    decimal EnteredDose,
    string EnteredUnit,
    string DisplayUnit,
    string Route,
    DateTimeOffset AdministeredAt,
    DateTimeOffset? MeatHoldUntil,
    DateTimeOffset? MilkHoldUntil,
    string? Notes);
