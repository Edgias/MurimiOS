using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MasterDataImportJob;

public enum MasterDataImportRowStatus
{
    Invalid = 1,
    Ready = 2,
    Applied = 3,
    Failed = 4
}
