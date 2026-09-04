using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateStandard;

public sealed record CreateComplianceStandardRequest(Guid? Id, Guid? SeriesId, string Code, string Name, string Scheme,
    string VersionLabel, int Revision, DateOnly EffectiveFrom, DateOnly? EffectiveTo, string? Jurisdiction);
