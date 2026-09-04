using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
public enum InfrastructureAssetStatus { Planned = 1, Active = 2, Inactive = 3, Decommissioned = 4 }
