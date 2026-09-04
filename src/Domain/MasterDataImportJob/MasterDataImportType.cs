using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MasterDataImportJob;

public enum MasterDataImportType
{
    Farms = 1,
    Fields = 2,
    Livestock = 3,
    Inventory = 4,
    Suppliers = 5,
    OpeningBalances = 6,
    FinanceTransactions = 7
}
