using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
public enum InfrastructureAssetType { Building = 1, Borehole = 2, Reservoir = 3, Irrigation = 4, Fence = 5, Road = 6, Power = 7, Storage = 8, LivestockFacility = 9, Processing = 10, Other = 11 }
