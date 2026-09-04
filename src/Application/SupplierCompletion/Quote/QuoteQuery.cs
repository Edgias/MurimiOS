using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.Quote;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;

public sealed record QuoteQuery(Guid contractId, decimal quantity, string? qualityGrade, DateOnly? effectiveOn) : IRequest<UseCaseResult>;
