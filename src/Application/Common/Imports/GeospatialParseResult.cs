using System.Buffers.Binary;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using NetTopologySuite;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Application.Common.Imports;

public sealed record GeospatialParseResult(
    IReadOnlyList<ParsedGeospatialFeature> Features,
    IReadOnlyList<GeospatialValidationIssue> FileIssues);
