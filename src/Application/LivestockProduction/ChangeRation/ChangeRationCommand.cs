using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ChangeRation;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using RationPlanStatus = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.RationPlanStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record ChangeRationCommand(Guid planId, string command) : IRequest<UseCaseResult>;
