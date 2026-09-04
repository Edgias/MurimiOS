using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.LandTenureRecord;
public enum LandTenureStatus { Active = 1, Ended = 2, Terminated = 3, Superseded = 4 }
