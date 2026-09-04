using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateMilk;
using MilkSession = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.MilkSession;
public sealed record CreateMilkRecordRequest(DateTimeOffset MilkedAt, MilkSession Session, decimal Quantity,
    decimal DiscardedQuantity, string Unit, decimal? FatPercent, decimal? ProteinPercent, string? QualityGrade, string? Notes);
