using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance;
using CloseCostCentreEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.CloseCostCentre.CloseCostCentreEndpoint;
using CreateCostCentreEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.CreateCostCentre.CreateCostCentreEndpoint;
using CreateTransactionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction.CreateTransactionEndpoint;
using GetAccountingImportEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetAccountingImport.GetAccountingImportEndpoint;
using GetEnterpriseProfitabilityEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability.GetEnterpriseProfitabilityEndpoint;
using GetFarmProfitabilityEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetFarmProfitability.GetFarmProfitabilityEndpoint;
using ImportAccountingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting.ImportAccountingEndpoint;
using ListCostCentresEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.ListCostCentres.ListCostCentresEndpoint;
using ListTransactionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.ListTransactions.ListTransactionsEndpoint;
using ReconcileAccountingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.ReconcileAccounting.ReconcileAccountingEndpoint;
using RecordLabourEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour.RecordLabourEndpoint;
using RecordMachineryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery.RecordMachineryEndpoint;
using ReverseTransactionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Finance.ReverseTransaction.ReverseTransactionEndpoint;


public static class FinanceEndpoints
{
    public static void MapFinanceEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder finance = api.MapGroup("/finance").WithTags("Finance and Profitability");
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.ListCostCentres.ListCostCentresEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.CreateCostCentre.CreateCostCentreEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.CloseCostCentre.CloseCostCentreEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.ListTransactions.ListTransactionsEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction.CreateTransactionEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.ReverseTransaction.ReverseTransactionEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour.RecordLabourEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery.RecordMachineryEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetFarmProfitability.GetFarmProfitabilityEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability.GetEnterpriseProfitabilityEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting.ImportAccountingEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.GetAccountingImport.GetAccountingImportEndpoint.Map(finance);
        global::EdgIQ.MurimiOS.API.Endpoints.Finance.ReconcileAccounting.ReconcileAccountingEndpoint.Map(finance);
    }
}
