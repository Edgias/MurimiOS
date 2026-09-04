using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction;
using OffspringOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.OffspringOutcome;
using LivestockSex = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSex;
public sealed record CreateOffspringLinkRequest(Guid SubjectId, OffspringOutcome Outcome, LivestockSex Sex,
    string? Breed, decimal? BirthWeight, string? BirthWeightUnit);
