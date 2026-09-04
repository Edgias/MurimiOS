using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.PutSettings;

public sealed record UpdateRegionalSettingsRequest(
    string CountryCode,
    string CurrencyCode,
    string Locale,
    string TimeZone,
    int CropYearStartMonth,
    string AreaUnit,
    string MassUnit,
    string VolumeUnit,
    string TemperatureUnit);
