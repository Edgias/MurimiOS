using System.ComponentModel;

namespace EdgIQ.MurimiOS.Domain.LivestockProduction;

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
