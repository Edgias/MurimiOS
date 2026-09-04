using System.Buffers.Binary;
using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using NetTopologySuite;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Application.Common.Imports;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;

using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;

public sealed class GeospatialImportParser
{
    public const string Version = "geo-import-v1";
    public const int MaximumFeatures = 5_000;
    public const int MaximumVerticesPerFeature = 100_000;
    private static readonly GeometryFactory Factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public GeospatialParseResult Parse(GeospatialImportFormat format, ReadOnlyMemory<byte> content)
    {
        if (content.IsEmpty) return Empty("empty_file", "The import file is empty.");
        try
        {
            List<ParsedGeospatialFeature> features = format switch
            {
                GeospatialImportFormat.GeoJson => ParseGeoJson(content.Span),
                GeospatialImportFormat.Kml => ParseKml(OpenXmlContent(content, ".kml")),
                GeospatialImportFormat.Shapefile => ParseShapefile(OpenShapeContent(content)),
                GeospatialImportFormat.IsoXml => ParseIsoXml(OpenXmlContent(content, "taskdata.xml")),
                _ => throw new GeospatialParseException("unsupported_format", "The requested geospatial format is not supported.")
            };
            if (features.Count > MaximumFeatures)
                return Empty("too_many_features", $"An import cannot contain more than {MaximumFeatures} features.");
            Validate(features);
            return new GeospatialParseResult(features, []);
        }
        catch (GeospatialParseException exception)
        {
            return Empty(exception.Code, exception.Message);
        }
        catch (Exception exception) when (exception is JsonException or InvalidDataException or System.Xml.XmlException or ArgumentException)
        {
            return Empty("malformed_file", exception.Message);
        }
    }

    private static List<ParsedGeospatialFeature> ParseGeoJson(ReadOnlySpan<byte> content)
    {
        using JsonDocument document = JsonDocument.Parse(content.ToArray());
        JsonElement root = document.RootElement;
        var results = new List<ParsedGeospatialFeature>();
        string type = PropertyString(root, "type") ?? string.Empty;
        if (type.Equals("FeatureCollection", StringComparison.OrdinalIgnoreCase))
        {
            if (!root.TryGetProperty("features", out JsonElement features) || features.ValueKind != JsonValueKind.Array)
                throw new GeospatialParseException("invalid_geojson", "A GeoJSON FeatureCollection requires a features array.");
            foreach (JsonElement feature in features.EnumerateArray()) AddGeoJsonFeature(feature, results);
        }
        else if (type.Equals("Feature", StringComparison.OrdinalIgnoreCase)) AddGeoJsonFeature(root, results);
        else
        {
            MultiPolygon geometry = ReadGeoJsonGeometry(root);
            results.Add(new ParsedGeospatialFeature(1, "Feature 1", geometry, "{}"));
        }
        return results;
    }

    private static void AddGeoJsonFeature(JsonElement feature, List<ParsedGeospatialFeature> results)
    {
        if (!feature.TryGetProperty("geometry", out JsonElement geometry) || geometry.ValueKind == JsonValueKind.Null)
            throw new GeospatialParseException("missing_geometry", $"Feature {results.Count + 1} has no geometry.");
        MultiPolygon boundary = ReadGeoJsonGeometry(geometry);
        string properties = feature.TryGetProperty("properties", out JsonElement value) && value.ValueKind == JsonValueKind.Object
            ? value.GetRawText() : "{}";
        string? name = value.ValueKind == JsonValueKind.Object
            ? PropertyString(value, "name") ?? PropertyString(value, "Name") ?? PropertyString(value, "field_name")
            : null;
        int ordinal = results.Count + 1;
        results.Add(new ParsedGeospatialFeature(ordinal, name ?? $"Feature {ordinal}", boundary, properties));
    }

    private static MultiPolygon ReadGeoJsonGeometry(JsonElement geometry)
    {
        string type = PropertyString(geometry, "type") ?? string.Empty;
        if (!geometry.TryGetProperty("coordinates", out JsonElement coordinates))
            throw new GeospatialParseException("missing_coordinates", "A GeoJSON geometry requires coordinates.");
        return type.ToLowerInvariant() switch
        {
            "polygon" => Factory.CreateMultiPolygon([ReadPolygon(coordinates)]),
            "multipolygon" => Factory.CreateMultiPolygon(coordinates.EnumerateArray().Select(ReadPolygon).ToArray()),
            _ => throw new GeospatialParseException("unsupported_geometry", "Only Polygon and MultiPolygon geometries can define fields.")
        };
    }

    private static Polygon ReadPolygon(JsonElement rings)
    {
        JsonElement[] values = rings.EnumerateArray().ToArray();
        if (values.Length == 0) throw new GeospatialParseException("empty_polygon", "A polygon requires an exterior ring.");
        LinearRing shell = ReadRing(values[0]);
        LinearRing[] holes = values.Skip(1).Select(ReadRing).ToArray();
        return Factory.CreatePolygon(shell, holes);
    }

    private static LinearRing ReadRing(JsonElement positions)
    {
        var coordinates = new List<Coordinate>();
        foreach (JsonElement position in positions.EnumerateArray())
        {
            JsonElement[] ordinates = position.EnumerateArray().ToArray();
            if (ordinates.Length < 2 || !ordinates[0].TryGetDouble(out double longitude) || !ordinates[1].TryGetDouble(out double latitude))
                throw new GeospatialParseException("invalid_coordinate", "Coordinates must contain numeric longitude and latitude values.");
            coordinates.Add(new Coordinate(longitude, latitude));
        }
        return CreateRing(coordinates);
    }

    private static List<ParsedGeospatialFeature> ParseKml(byte[] xml)
    {
        XDocument document = LoadXml(xml);
        var results = new List<ParsedGeospatialFeature>();
        foreach (XElement polygonElement in document.Descendants().Where(element => element.Name.LocalName == "Polygon"))
        {
            XElement? placemark = polygonElement.Ancestors().FirstOrDefault(element => element.Name.LocalName == "Placemark");
            string name = placemark?.Elements().FirstOrDefault(element => element.Name.LocalName == "name")?.Value?.Trim()
                ?? $"Feature {results.Count + 1}";
            XElement? outer = polygonElement.Descendants().FirstOrDefault(element => element.Name.LocalName == "outerBoundaryIs")
                ?.Descendants().FirstOrDefault(element => element.Name.LocalName == "coordinates");
            if (outer is null) continue;
            LinearRing shell = ReadCoordinateText(outer.Value);
            LinearRing[] holes = polygonElement.Descendants().Where(element => element.Name.LocalName == "innerBoundaryIs")
                .Select(element => element.Descendants().FirstOrDefault(child => child.Name.LocalName == "coordinates"))
                .Where(element => element is not null).Select(element => ReadCoordinateText(element!.Value)).ToArray();
            var boundary = Factory.CreateMultiPolygon([Factory.CreatePolygon(shell, holes)]);
            results.Add(new ParsedGeospatialFeature(results.Count + 1, name, boundary, "{}"));
        }
        return results;
    }

    private static LinearRing ReadCoordinateText(string text)
    {
        var coordinates = new List<Coordinate>();
        foreach (string tuple in text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            string[] values = tuple.Split(',');
            if (values.Length < 2
                || !double.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude)
                || !double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude))
                throw new GeospatialParseException("invalid_coordinate", "A KML coordinate could not be parsed.");
            coordinates.Add(new Coordinate(longitude, latitude));
        }
        return CreateRing(coordinates);
    }

    private static List<ParsedGeospatialFeature> ParseShapefile(byte[] bytes)
    {
        if (bytes.Length < 100 || BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(0, 4)) != 9994)
            throw new GeospatialParseException("invalid_shapefile", "The .shp header is missing or invalid.");
        int fileShapeType = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(32, 4));
        if (fileShapeType is not (5 or 15 or 25))
            throw new GeospatialParseException("unsupported_shape_type", "Only Polygon shapefiles are supported.");
        var results = new List<ParsedGeospatialFeature>();
        int offset = 100;
        while (offset + 8 <= bytes.Length)
        {
            int contentBytes = checked(BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(offset + 4, 4)) * 2);
            offset += 8;
            if (contentBytes < 4 || offset + contentBytes > bytes.Length)
                throw new GeospatialParseException("invalid_shapefile_record", "A shapefile record length is invalid.");
            ReadOnlySpan<byte> record = bytes.AsSpan(offset, contentBytes);
            offset += contentBytes;
            int shapeType = BinaryPrimitives.ReadInt32LittleEndian(record[..4]);
            if (shapeType == 0) continue;
            if (shapeType is not (5 or 15 or 25) || record.Length < 44)
                throw new GeospatialParseException("unsupported_shape_type", "The shapefile contains a non-polygon record.");
            int partCount = BinaryPrimitives.ReadInt32LittleEndian(record.Slice(36, 4));
            int pointCount = BinaryPrimitives.ReadInt32LittleEndian(record.Slice(40, 4));
            int pointsOffset = 44 + checked(partCount * 4);
            if (partCount < 1 || pointCount < 4 || pointsOffset + checked(pointCount * 16) > record.Length)
                throw new GeospatialParseException("invalid_shapefile_record", "A shapefile polygon record is truncated.");
            int[] starts = new int[partCount];
            for (int index = 0; index < partCount; index++)
                starts[index] = BinaryPrimitives.ReadInt32LittleEndian(record.Slice(44 + index * 4, 4));
            var rings = new List<LinearRing>();
            for (int part = 0; part < partCount; part++)
            {
                int start = starts[part];
                int end = part + 1 < partCount ? starts[part + 1] : pointCount;
                if (start < 0 || end <= start || end > pointCount)
                    throw new GeospatialParseException("invalid_shapefile_parts", "A shapefile part index is invalid.");
                var coordinates = new List<Coordinate>(end - start);
                for (int point = start; point < end; point++)
                {
                    int position = pointsOffset + point * 16;
                    coordinates.Add(new Coordinate(
                        BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(record.Slice(position, 8))),
                        BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(record.Slice(position + 8, 8)))));
                }
                rings.Add(CreateRing(coordinates));
            }
            MultiPolygon boundary = BuildShapePolygons(rings);
            int ordinal = results.Count + 1;
            results.Add(new ParsedGeospatialFeature(ordinal, $"Shape {ordinal}", boundary, "{}"));
        }
        return results;
    }

    private static MultiPolygon BuildShapePolygons(List<LinearRing> rings)
    {
        List<LinearRing> shells = rings.Where(ring => !Orientation.IsCCW(ring.CoordinateSequence)).ToList();
        List<LinearRing> holes = rings.Where(ring => Orientation.IsCCW(ring.CoordinateSequence)).ToList();
        if (shells.Count == 0) { shells.AddRange(rings); holes.Clear(); }
        var polygons = new List<Polygon>();
        foreach (LinearRing shell in shells)
        {
            Polygon shellPolygon = Factory.CreatePolygon(shell);
            LinearRing[] containedHoles = holes.Where(hole => shellPolygon.Covers(Factory.CreatePoint(hole.GetCoordinateN(0)))).ToArray();
            polygons.Add(Factory.CreatePolygon(shell, containedHoles));
        }
        return Factory.CreateMultiPolygon(polygons.ToArray());
    }

    private static List<ParsedGeospatialFeature> ParseIsoXml(byte[] xml)
    {
        XDocument document = LoadXml(xml);
        XElement[] partfields = document.Descendants().Where(element => element.Name.LocalName.Equals("PFD", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (partfields.Length == 0) partfields = [document.Root ?? throw new GeospatialParseException("invalid_isoxml", "ISOXML has no root element.")];
        var results = new List<ParsedGeospatialFeature>();
        foreach (XElement partfield in partfields)
        {
            string name = AttributeValue(partfield, "D", "name", "Name") ?? $"Partfield {results.Count + 1}";
            XElement[] polygonElements = partfield.Descendants().Where(element => element.Name.LocalName.Equals("PLN", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (polygonElements.Length == 0) polygonElements = [partfield];
            var polygons = new List<Polygon>();
            foreach (XElement polygon in polygonElements)
            {
                var coordinates = new List<Coordinate>();
                foreach (XElement point in polygon.Descendants().Where(element => element.Name.LocalName.Equals("PNT", StringComparison.OrdinalIgnoreCase)))
                {
                    string? northText = AttributeValue(point, "C", "latitude", "Latitude", "north", "North");
                    string? eastText = AttributeValue(point, "D", "longitude", "Longitude", "east", "East");
                    if (!double.TryParse(northText, NumberStyles.Float, CultureInfo.InvariantCulture, out double north)
                        || !double.TryParse(eastText, NumberStyles.Float, CultureInfo.InvariantCulture, out double east)) continue;
                    if (Math.Abs(north) > 90) north /= 10_000_000d;
                    if (Math.Abs(east) > 180) east /= 10_000_000d;
                    coordinates.Add(new Coordinate(east, north));
                }
                if (coordinates.Count >= 3) polygons.Add(Factory.CreatePolygon(CreateRing(coordinates)));
            }
            if (polygons.Count == 0) continue;
            results.Add(new ParsedGeospatialFeature(results.Count + 1, name,
                Factory.CreateMultiPolygon(polygons.ToArray()), "{}"));
        }
        return results;
    }

    private static void Validate(List<ParsedGeospatialFeature> features)
    {
        foreach (ParsedGeospatialFeature feature in features)
        {
            feature.Boundary.SRID = 4326;
            if (feature.Boundary.NumPoints > MaximumVerticesPerFeature)
                feature.Issues.Add(new("too_many_vertices", "error", $"A feature cannot contain more than {MaximumVerticesPerFeature} vertices."));
            if (feature.Boundary.Coordinates.Any(coordinate => !double.IsFinite(coordinate.X) || !double.IsFinite(coordinate.Y)
                    || coordinate.X is < -180 or > 180 || coordinate.Y is < -90 or > 90))
                feature.Issues.Add(new("coordinate_out_of_range", "error", "Coordinates must be WGS84 longitude/latitude values."));
            if (!feature.Boundary.IsValid)
                feature.Issues.Add(new("invalid_topology", "error", "The polygon topology is invalid and must be corrected before import."));
            if (feature.Boundary.IsEmpty || feature.Boundary.Area <= 0)
                feature.Issues.Add(new("empty_geometry", "error", "The field boundary has no area."));
        }

        var ordered = features.OrderBy(feature => feature.Boundary.EnvelopeInternal.MinX).ToArray();
        for (int left = 0; left < ordered.Length; left++)
        {
            Envelope envelope = ordered[left].Boundary.EnvelopeInternal;
            for (int right = left + 1; right < ordered.Length && ordered[right].Boundary.EnvelopeInternal.MinX <= envelope.MaxX; right++)
            {
                if (!envelope.Intersects(ordered[right].Boundary.EnvelopeInternal)) continue;
                Geometry intersection = ordered[left].Boundary.Intersection(ordered[right].Boundary);
                if (intersection.Area <= 1e-12) continue;
                ordered[left].Issues.Add(new("overlapping_feature", "warning", "This boundary overlaps another imported feature.", ordered[right].Ordinal));
                ordered[right].Issues.Add(new("overlapping_feature", "warning", "This boundary overlaps another imported feature.", ordered[left].Ordinal));
            }
        }
    }

    private static LinearRing CreateRing(List<Coordinate> coordinates)
    {
        if (coordinates.Count > 0 && !coordinates[0].Equals2D(coordinates[^1])) coordinates.Add(coordinates[0].Copy());
        if (coordinates.Count < 4 || coordinates.Select(value => (value.X, value.Y)).Distinct().Count() < 3)
            throw new GeospatialParseException("invalid_ring", "A polygon ring requires at least three distinct coordinates.");
        return Factory.CreateLinearRing(coordinates.ToArray());
    }

    private static byte[] OpenXmlContent(ReadOnlyMemory<byte> content, string preferredName)
    {
        if (!IsZip(content.Span)) return content.ToArray();
        using var archive = new ZipArchive(new MemoryStream(content.ToArray()), ZipArchiveMode.Read);
        ZipArchiveEntry? entry = archive.Entries.FirstOrDefault(item => item.FullName.EndsWith(preferredName, StringComparison.OrdinalIgnoreCase))
            ?? archive.Entries.FirstOrDefault(item => item.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
        if (entry is null) throw new GeospatialParseException("missing_xml", $"The archive does not contain {preferredName}.");
        return ReadEntry(entry);
    }

    private static byte[] OpenShapeContent(ReadOnlyMemory<byte> content)
    {
        if (!IsZip(content.Span)) return content.ToArray();
        using var archive = new ZipArchive(new MemoryStream(content.ToArray()), ZipArchiveMode.Read);
        ZipArchiveEntry? entry = archive.Entries.FirstOrDefault(item => item.FullName.EndsWith(".shp", StringComparison.OrdinalIgnoreCase));
        if (entry is null) throw new GeospatialParseException("missing_shp", "The archive does not contain a .shp file.");
        return ReadEntry(entry);
    }

    private static byte[] ReadEntry(ZipArchiveEntry entry)
    {
        if (entry.Length > 52_428_800) throw new GeospatialParseException("archive_entry_too_large", "An archived import entry exceeds 50 MiB.");
        using Stream source = entry.Open();
        using var destination = new MemoryStream();
        source.CopyTo(destination);
        return destination.ToArray();
    }

    private static XDocument LoadXml(byte[] xml)
    {
        using var reader = System.Xml.XmlReader.Create(new MemoryStream(xml), new System.Xml.XmlReaderSettings
        {
            DtdProcessing = System.Xml.DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = 52_428_800
        });
        return XDocument.Load(reader, LoadOptions.None);
    }

    private static bool IsZip(ReadOnlySpan<byte> content) => content.Length >= 4 && content[0] == 0x50 && content[1] == 0x4b;
    private static string? PropertyString(JsonElement element, string name) =>
        element.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    private static string? AttributeValue(XElement element, params string[] names) => element.Attributes()
        .FirstOrDefault(attribute => names.Contains(attribute.Name.LocalName, StringComparer.OrdinalIgnoreCase))?.Value;
    private static GeospatialParseResult Empty(string code, string message) =>
        new([], [new GeospatialValidationIssue(code, "error", message)]);
}
