using System.Globalization;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.TenantRegionalSettings;

public sealed class TenantRegionalSettings : TenantEntity
{
    private TenantRegionalSettings() { }

    public TenantRegionalSettings(
        Guid id,
        string countryCode,
        string currencyCode,
        string locale,
        string timeZone,
        int cropYearStartMonth,
        string areaUnit,
        string massUnit,
        string volumeUnit,
        string temperatureUnit) : base(id)
    {
        Update(countryCode, currencyCode, locale, timeZone, cropYearStartMonth,
            areaUnit, massUnit, volumeUnit, temperatureUnit);
    }

    public string CountryCode { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public string Locale { get; private set; } = string.Empty;
    public string TimeZone { get; private set; } = string.Empty;
    public int CropYearStartMonth { get; private set; }
    public string AreaUnit { get; private set; } = string.Empty;
    public string MassUnit { get; private set; } = string.Empty;
    public string VolumeUnit { get; private set; } = string.Empty;
    public string TemperatureUnit { get; private set; } = string.Empty;

    public void Update(
        string countryCode,
        string currencyCode,
        string locale,
        string timeZone,
        int cropYearStartMonth,
        string areaUnit,
        string massUnit,
        string volumeUnit,
        string temperatureUnit)
    {
        CountryCode = NormalizeCountry(countryCode);
        CurrencyCode = new CurrencyCode(currencyCode).Value;
        Locale = NormalizeLocale(locale);
        TimeZone = RegionalTime.ResolveTimeZone(timeZone).Id;
        if (cropYearStartMonth is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(cropYearStartMonth), "Crop-year start month must be between 1 and 12.");
        CropYearStartMonth = cropYearStartMonth;
        AreaUnit = MeasurementUnits.Normalize(areaUnit, UnitDimension.Area);
        MassUnit = MeasurementUnits.Normalize(massUnit, UnitDimension.Mass);
        VolumeUnit = MeasurementUnits.Normalize(volumeUnit, UnitDimension.Volume);
        TemperatureUnit = MeasurementUnits.Normalize(temperatureUnit, UnitDimension.Temperature);
    }

    private static string NormalizeCountry(string value)
    {
        string normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalized.Length != 2 || normalized.Any(character => !char.IsAsciiLetter(character)))
            throw new ArgumentException("A two-letter ISO 3166-1 country code is required.", nameof(value));
        return normalized;
    }

    private static string NormalizeLocale(string value)
    {
        string normalized = value?.Trim() ?? string.Empty;
        try
        {
            return CultureInfo.GetCultureInfo(normalized).Name;
        }
        catch (CultureNotFoundException exception)
        {
            throw new ArgumentException($"Unknown locale '{normalized}'.", nameof(value), exception);
        }
    }
}
