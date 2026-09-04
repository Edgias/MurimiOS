using System.ComponentModel;

namespace EdgIQ.MurimiOS.Domain.LivestockProduction;

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
