using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateContract;
public sealed record CreateSupplyContractRequest(Guid? Id, string ContractNumber, Guid? RelationshipId, Guid? HostedSupplierId,
    string Commodity, DateOnly ValidFrom, DateOnly ValidTo, decimal ContractedQuantity, string QuantityUnit,
    decimal? UnitPrice, string? Currency, JsonElement? Terms);
