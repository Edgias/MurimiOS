using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateAllocation;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;
public sealed record CreateContractAllocationRequest(Guid? Id, string Reference, DateOnly DeliveryFrom, DateOnly DeliveryTo,
    decimal AllocatedQuantity, string QuantityUnit);
