using System.Buffers.Binary;
using System.Text;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Application.Common.Imports;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Application.UnitTests;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using GeospatialImportParser = global::EdgIQ.MurimiOS.Application.Common.Imports.GeospatialImportParser;
using GeospatialParseResult = global::EdgIQ.MurimiOS.Application.Common.Imports.GeospatialParseResult;

public sealed class GeospatialImportTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T12:00:00Z");
    private readonly GeospatialImportParser parser = new();

    [Fact]
    public void GeoJson_parser_preserves_properties_and_warns_for_overlaps()
    {
        byte[] json = Encoding.UTF8.GetBytes("""
            {"type":"FeatureCollection","features":[
              {"type":"Feature","properties":{"name":"North"},"geometry":{"type":"Polygon","coordinates":[[[31,-18],[31.01,-18],[31.01,-17.99],[31,-17.99],[31,-18]]]}},
              {"type":"Feature","properties":{"name":"South"},"geometry":{"type":"Polygon","coordinates":[[[31.005,-18],[31.015,-18],[31.015,-17.99],[31.005,-17.99],[31.005,-18]]]}}
            ]}
            """);

        GeospatialParseResult result = parser.Parse(GeospatialImportFormat.GeoJson, json);

        Assert.Empty(result.FileIssues);
        Assert.Equal(2, result.Features.Count);
        Assert.All(result.Features, feature => Assert.Contains(feature.Issues, issue => issue.Code == "overlapping_feature"));
        Assert.Equal("North", result.Features[0].Name);
    }

    [Fact]
    public void Kml_parser_reads_polygon_and_closes_ring()
    {
        byte[] kml = Encoding.UTF8.GetBytes("""
            <kml xmlns="http://www.opengis.net/kml/2.2"><Document><Placemark><name>East Block</name>
            <Polygon><outerBoundaryIs><LinearRing><coordinates>31,-18 31.01,-18 31.01,-17.99 31,-17.99</coordinates>
            </LinearRing></outerBoundaryIs></Polygon></Placemark></Document></kml>
            """);

        GeospatialParseResult result = parser.Parse(GeospatialImportFormat.Kml, kml);

        Assert.Empty(result.FileIssues);
        Assert.Single(result.Features);
        Assert.Equal("East Block", result.Features[0].Name);
        Assert.True(result.Features[0].Boundary.IsValid);
    }

    [Fact]
    public void IsoXml_parser_reads_partfield_points()
    {
        byte[] xml = Encoding.UTF8.GetBytes("""
            <ISO11783_TaskData><PFD A="PFD1" D="Maize West"><PLN><LSG>
            <PNT C="-18" D="31"/><PNT C="-18" D="31.01"/><PNT C="-17.99" D="31.01"/><PNT C="-17.99" D="31"/>
            </LSG></PLN></PFD></ISO11783_TaskData>
            """);

        GeospatialParseResult result = parser.Parse(GeospatialImportFormat.IsoXml, xml);

        Assert.Empty(result.FileIssues);
        Assert.Single(result.Features);
        Assert.Equal("Maize West", result.Features[0].Name);
    }

    [Fact]
    public void Shapefile_parser_reads_polygon_records()
    {
        GeospatialParseResult result = parser.Parse(GeospatialImportFormat.Shapefile, BuildShapefile());

        Assert.Empty(result.FileIssues);
        Assert.Single(result.Features);
        Assert.Equal(5, result.Features[0].Boundary.NumPoints);
    }

    [Fact]
    public void Parser_rejects_non_polygon_geojson()
    {
        byte[] point = Encoding.UTF8.GetBytes("{\"type\":\"Point\",\"coordinates\":[31,-18]}");

        GeospatialParseResult result = parser.Parse(GeospatialImportFormat.GeoJson, point);

        Assert.Contains(result.FileIssues, issue => issue.Code == "unsupported_geometry");
    }

    [Fact]
    public void Import_job_requires_validation_before_apply()
    {
        var job = NewJob();
        Assert.Throws<InvalidOperationException>(() => job.MarkApplied("owner", Now));

        job.CompleteValidation(1, 1, 0, 0, Now);
        job.MarkApplied("owner", Now.AddMinutes(1));

        Assert.Equal(GeospatialImportStatus.Applied, job.Status);
        Assert.Equal("owner", job.AppliedBy);
    }

    [Fact]
    public void Import_feature_can_only_be_applied_once()
    {
        var feature = new GeospatialImportFeature(Guid.NewGuid(), Guid.NewGuid(), 1, "Field",
            Boundary(), "{}", "[]", true);
        Guid fieldId = Guid.NewGuid();
        Guid boundaryId = Guid.NewGuid();

        feature.MarkApplied(fieldId, boundaryId, Now);

        Assert.Equal(fieldId, feature.AppliedFieldId);
        Assert.Throws<InvalidOperationException>(() => feature.MarkApplied(Guid.NewGuid(), Guid.NewGuid(), Now));
    }

    [Fact]
    public void Import_permissions_separate_preview_from_application()
    {
        Assert.Contains(PlatformPermissions.ImportsManage, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.ImportsRead, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.ImportsManage, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.Contains(PlatformPermissions.ImportsRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.ImportsRead, PlatformPermissions.ForRole(TenantRole.Buyer));
    }

    private static GeospatialImportJob NewJob() => new(Guid.NewGuid(), Guid.NewGuid(), GeospatialImportFormat.GeoJson,
        "fields.geojson", "application/geo+json", 100, new string('a', 64), "tenant/imports/job", GeospatialImportParser.Version);

    private static MultiPolygon Boundary()
    {
        GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        return factory.CreateMultiPolygon([factory.CreatePolygon([
            new Coordinate(31, -18), new Coordinate(31.01, -18), new Coordinate(31.01, -17.99),
            new Coordinate(31, -17.99), new Coordinate(31, -18)])]);
    }

    private static byte[] BuildShapefile()
    {
        Coordinate[] points = Boundary().Coordinates;
        int contentBytes = 44 + 4 + points.Length * 16;
        byte[] bytes = new byte[100 + 8 + contentBytes];
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(0, 4), 9994);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(24, 4), bytes.Length / 2);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(28, 4), 1000);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(32, 4), 5);
        WriteBox(bytes.AsSpan(36, 32));
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(100, 4), 1);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(104, 4), contentBytes / 2);
        Span<byte> record = bytes.AsSpan(108);
        BinaryPrimitives.WriteInt32LittleEndian(record[..4], 5);
        WriteBox(record.Slice(4, 32));
        BinaryPrimitives.WriteInt32LittleEndian(record.Slice(36, 4), 1);
        BinaryPrimitives.WriteInt32LittleEndian(record.Slice(40, 4), points.Length);
        BinaryPrimitives.WriteInt32LittleEndian(record.Slice(44, 4), 0);
        for (int index = 0; index < points.Length; index++)
        {
            WriteDouble(record.Slice(48 + index * 16, 8), points[index].X);
            WriteDouble(record.Slice(56 + index * 16, 8), points[index].Y);
        }
        return bytes;
    }

    private static void WriteBox(Span<byte> target)
    {
        WriteDouble(target[..8], 31); WriteDouble(target.Slice(8, 8), -18);
        WriteDouble(target.Slice(16, 8), 31.01); WriteDouble(target.Slice(24, 8), -17.99);
    }

    private static void WriteDouble(Span<byte> target, double value) =>
        BinaryPrimitives.WriteInt64LittleEndian(target, BitConverter.DoubleToInt64Bits(value));
}
