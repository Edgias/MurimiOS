using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateMedicine;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;

public sealed record CreateMedicineCommand(Guid farmId, CreateVeterinaryMedicineRequest request) : IRequest<UseCaseResult>;
