using System.Buffers.Binary;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using NetTopologySuite;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Application.Common.Imports;

public sealed class ParsedGeospatialFeature(int ordinal, string name, MultiPolygon boundary, string propertiesJson)
{
    public int Ordinal { get; } = ordinal;
    public string Name { get; } = name;
    public MultiPolygon Boundary { get; } = boundary;
    public string PropertiesJson { get; } = propertiesJson;
    public List<GeospatialValidationIssue> Issues { get; } = [];
    public bool IsEligible => Issues.All(issue => issue.Severity != "error");
}
