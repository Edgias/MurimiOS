using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.AddResource;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;
public sealed record AddWorkOrderResourceRequest(Guid? Id, WorkResourceType ResourceType, string ResourceName,
    decimal Quantity, string Unit, decimal UnitCostPerCanonicalUnit, string Currency, Guid? AgriculturalInputId = null,
    Guid? StockLotId = null, string? ResourceReference = null);
