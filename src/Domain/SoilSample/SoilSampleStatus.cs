using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SoilSample;
public enum SoilSampleStatus { Collected = 1, SubmittedToLaboratory = 2, Analysed = 3, Rejected = 4 }
