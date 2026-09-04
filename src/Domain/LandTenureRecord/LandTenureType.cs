using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.LandTenureRecord;

public enum LandTenureType { Freehold = 1, Leasehold = 2, Customary = 3, Licence = 4, Sharecropping = 5, Other = 6 }
