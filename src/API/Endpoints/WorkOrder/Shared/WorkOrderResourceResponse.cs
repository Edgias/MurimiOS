using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;
public sealed record WorkOrderResourceResponse(Guid Id, WorkResourceType ResourceType, string ResourceName,
    string? ResourceReference, Guid? AgriculturalInputId, Guid? StockLotId, decimal Quantity, string Unit,
    decimal EnteredQuantity, string EnteredUnit, decimal UnitCostPerCanonicalUnit, string Currency, decimal PlannedCost,
    WorkResourceStatus Status);
