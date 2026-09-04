using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CloseCompany;
public sealed record CloseLegalCompanyRequest(DateOnly EffectiveTo);
