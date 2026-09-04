using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth;
using CreateDispatchEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateDispatch.CreateDispatchEndpoint;
using CreateMedicineEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateMedicine.CreateMedicineEndpoint;
using CreateStockLotEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateStockLot.CreateStockLotEndpoint;
using GetDispatchEligibilityEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.GetDispatchEligibility.GetDispatchEligibilityEndpoint;
using ListMedicinesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListMedicines.ListMedicinesEndpoint;
using ListStockLotsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListStockLots.ListStockLotsEndpoint;
using ListTreatmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListTreatments.ListTreatmentsEndpoint;


public static class LivestockHealthEndpoints
{
    public static void MapLivestockHealthEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListMedicines.ListMedicinesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateMedicine.CreateMedicineEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListStockLots.ListStockLotsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateStockLot.CreateStockLotEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListTreatments.ListTreatmentsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.GetDispatchEligibility.GetDispatchEligibilityEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateDispatch.CreateDispatchEndpoint.Map(api);
    }
}
