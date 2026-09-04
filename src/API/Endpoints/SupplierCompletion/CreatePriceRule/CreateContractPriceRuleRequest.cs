using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.CreatePriceRule;
using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;
public sealed record CreateContractPriceRuleRequest(Guid? Id, int Priority, ContractPricingMethod Method,
    decimal MinimumQuantity, decimal? MaximumQuantity, string? QualityGrade, decimal UnitPrice,
    string Currency, DateOnly EffectiveFrom, DateOnly EffectiveTo);
