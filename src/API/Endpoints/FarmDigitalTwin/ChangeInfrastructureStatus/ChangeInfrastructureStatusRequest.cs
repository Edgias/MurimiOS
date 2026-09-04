using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeInfrastructureStatus;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
public sealed record ChangeInfrastructureStatusRequest(InfrastructureAssetStatus Status);
