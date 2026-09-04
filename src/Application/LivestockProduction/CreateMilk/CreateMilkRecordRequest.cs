using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateMilk;

using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using MilkSession = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.MilkSession;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record CreateMilkRecordRequest(DateTimeOffset MilkedAt, MilkSession Session, decimal Quantity,
    decimal DiscardedQuantity, string Unit, decimal? FatPercent, decimal? ProteinPercent, string? QualityGrade, string? Notes);
