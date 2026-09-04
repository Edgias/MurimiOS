namespace EdgIQ.MurimiOS.Domain.UnitMeasurement;
using UnitGroup = global::EdgIQ.MurimiOS.Domain.UnitGroup.UnitGroup;

public class UnitMeasurement(string name, Guid unitGroupId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid UnitGroupId { get; private set; } = unitGroupId;

    public UnitGroup UnitGroup { get; private set; } = default!;

    public void Update(string name, Guid unitGroupId)
    {
        Name = name;
        UnitGroupId = unitGroupId;
    }

}


