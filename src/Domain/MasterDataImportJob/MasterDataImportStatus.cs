using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MasterDataImportJob;

public enum MasterDataImportStatus
{
    Uploaded = 1,
    Validated = 2,
    Rejected = 3,
    Applying = 4,
    Applied = 5,
    Failed = 6
}
