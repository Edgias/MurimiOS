using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public sealed class LivestockBiologicalProfile : TenantEntity
{
    private LivestockBiologicalProfile() { }
    public LivestockBiologicalProfile(Guid id, Guid subjectId, LivestockSex sex, DateOnly? bornOn,
        string? breed, Guid? damId, Guid? sireId) : base(id)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("A livestock subject is required.", nameof(subjectId));
        if (!Enum.IsDefined(sex)) throw new ArgumentOutOfRangeException(nameof(sex));
        if (damId == Guid.Empty || sireId == Guid.Empty || damId == subjectId || sireId == subjectId
            || (damId.HasValue && sireId.HasValue && damId == sireId))
            throw new ArgumentException("Parent identifiers must be distinct, non-empty subjects.");
        SubjectId = subjectId; Sex = sex; BornOn = bornOn; Breed = Optional(breed, 120); DamId = damId; SireId = sireId;
    }
    public Guid SubjectId { get; private set; }
    public LivestockSex Sex { get; private set; }
    public DateOnly? BornOn { get; private set; }
    public string? Breed { get; private set; }
    public Guid? DamId { get; private set; }
    public Guid? SireId { get; private set; }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
