using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateAllocation;
public sealed record CreateContractAllocationRequest(Guid? Id, string Reference, DateOnly DeliveryFrom, DateOnly DeliveryTo,
    decimal AllocatedQuantity, string QuantityUnit);
