using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Reconcile;
public sealed record ReconcileContractRequest(Guid? Id, string Reference, decimal AcceptedQuantity,
    decimal RejectedQuantity, string? QualityGrade, decimal QualityAdjustmentAmount,
    decimal OtherAdjustmentAmount, DateOnly? EffectiveOn, string? Notes);
