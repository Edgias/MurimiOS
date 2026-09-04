using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ApproveRule;
public sealed record ApproveEliminationRuleRequest(string ApprovalReference);
