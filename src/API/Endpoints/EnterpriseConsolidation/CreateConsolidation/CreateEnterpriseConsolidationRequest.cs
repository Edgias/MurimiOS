using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateConsolidation;
public sealed record CreateEnterpriseConsolidationRequest(Guid? Id, DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd, string Currency);
