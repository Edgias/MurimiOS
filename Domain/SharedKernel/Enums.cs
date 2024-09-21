using System.ComponentModel;

namespace Edgias.MurimiOS.Domain.SharedKernel;

public enum Gender
{
    Male,
    Female
}

public enum ContactType
{
    Auditor,
    Breeder,
    Buyer,
    Certifier,
    Contact,
    Consultant,
    Contractor,
    Customer,
    Employee,
    Purchaser,
    Supplier,
    Vendor,
    Veterinarian
}

public enum LivestockType
{
    Alpaca,
    Bees,
    Bison,
    Buffalo,
    Butterflies,
    Camel,
    Cat,
    Catfish,
    Cattle,
    Chicken,
    Crickets,
    Deer,
    Dog,
    Donkey,
    Duck,
    Elk,
    Emu,
    Fish,
    Gayal,
    Goat,
    Goose,
    Guineafowl,
    Horse,
    Llama,
    Mealworms,
    Mollust,
    Mulle,
    Muskox,
    Ostrich,
    Patridge,
    Peafowl,
    Pheasant,
    Pig,
    Pigeon,
    Pony,
    Quail,
    Rabbit,
    Reindeer,
    Rhea,
    Salmon,
    Sheep,
    Shellfish,
    Silkworms,
    Swine,
    Tilapia,
    Trout,
    Turkey,
    [Description("Water Buffalo")]
    WaterBuffalo,
    Waxworms,
    Yak,
    Zeb
}

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

public enum LivestockUnitOfMeasure
{
    Bales,
    Barrels,
    Bunches,
    Bushels,
    Dozen,
    Grams,
    Head,
    Kilograms,
    Kiloliter,
    Liter,
    Milliliter,
    Tonnes
}

public enum TagLocation
{
    LeftEar,
    RightEar
}

public enum TreatmentApplicationMethod
{
    [Description("Intramuscular (in the muscle)")]
    Intramuscular,
    [Description("Intramammary (in the udder)")]
    Intramammary,
    [Description("Intrauterine (in the uterus)")]
    Intrauterine,
    [Description("Intravenous (in the vein)")]
    Intravenous,
    [Description("Oral (in the mouth)")]
    Oral,
    [Description("Subcutaneous (under the skin)")]
    Subcutaneous,
    [Description("Topical (on the skin)")]
    Topical,
    Other
}

public enum TreatmentType
{
    [Description("Alternative Therapy")]
    AlternativeTherapy,
    [Description("Artificial Insemination")]
    ArtificialInsemination,
    Branding,
    Castration,
    Dehorning,
    [Description("Dental Procedure")]
    DentalProcedure,
    Deworming,
    [Description("Ear Notching")]
    EarNotching,
    Euthanasia,
    [Description("Fly Treatment")]
    FlyTreatment,
    Grooming,
    [Description("Hoof Trim")]
    HoofTrim,
    Medication,
    Mites,
    [Description("Parasite Treatment")]
    ParasiteTreatment,
    [Description("Surgical Procedure")]
    SurgicalProcedure,
    Tagging,
    Vaccination,
    [Description("Other Procedure")]
    OtherProcedure
}


