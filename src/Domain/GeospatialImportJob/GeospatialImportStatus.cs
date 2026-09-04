using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.GeospatialImportJob;

public enum GeospatialImportStatus
{
    Uploaded = 1,
    Validated = 2,
    Rejected = 3,
    Applied = 4,
    Failed = 5
}
