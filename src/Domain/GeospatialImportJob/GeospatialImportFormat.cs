using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.GeospatialImportJob;

public enum GeospatialImportFormat
{
    GeoJson = 1,
    Kml = 2,
    Shapefile = 3,
    IsoXml = 4
}
