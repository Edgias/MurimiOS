using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateInfrastructure;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
using InfrastructureAssetType = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetType;
public sealed record CreateInfrastructureAssetRequest(Guid? Id, Guid? FieldId, string Code, string Name,
    InfrastructureAssetType Type, InfrastructureAssetStatus Status, double? Longitude, double? Latitude,
    DateOnly? InstalledOn, decimal? Capacity, string? CapacityUnit, JsonElement? Details);
