using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeAllocation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;

public sealed record ChangeAllocationCommand(Guid contractId, Guid allocationId, string command, AllocationCommandRequest request) : IRequest<UseCaseResult>;
