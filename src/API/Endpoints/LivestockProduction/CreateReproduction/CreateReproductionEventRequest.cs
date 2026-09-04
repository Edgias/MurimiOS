using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction;
using ReproductionEventType = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionEventType;
using ReproductionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionOutcome;
public sealed record CreateReproductionEventRequest(ReproductionEventType Type, ReproductionOutcome Outcome,
    DateTimeOffset OccurredAt, Guid? RelatedSubjectId, Guid? BreedingEventId, string? Method,
    DateOnly? ExpectedDueOn, int? OffspringCount, decimal? Weight, string? WeightUnit,
    string? Practitioner, string? Notes, IReadOnlyList<CreateOffspringLinkRequest> Offspring);
