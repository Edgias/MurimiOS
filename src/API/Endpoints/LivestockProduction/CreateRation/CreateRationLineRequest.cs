using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation;
public sealed record CreateRationLineRequest(Guid? FeedBatchId, string FeedName, decimal QuantityPerHeadPerDay,
    string Unit, decimal DryMatterPercent);
