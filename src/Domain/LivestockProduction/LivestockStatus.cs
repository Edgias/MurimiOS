using System.ComponentModel;

namespace EdgIQ.MurimiOS.Domain.LivestockProduction;

public enum LivestockStatus
{
    Active,
    Butchered,
    Culled,
    Deceased,
    Dry,
    Fishing,
    [Description("For Sale")]
    ForSale,
    Lactating,
    Lost,
    [Description("Off Farm")]
    OffFarm,
    Quarantined,
    Reference,
    Sick,
    Sold,
    Weaning,
    Archived
}
