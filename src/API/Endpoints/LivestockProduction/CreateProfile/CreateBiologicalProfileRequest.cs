using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateProfile;
using LivestockSex = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSex;

public sealed record CreateBiologicalProfileRequest(LivestockSex Sex, DateOnly? BornOn, string? Breed, Guid? DamId, Guid? SireId);
