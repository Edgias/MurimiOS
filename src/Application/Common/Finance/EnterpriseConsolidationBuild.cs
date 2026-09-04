using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Finance;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;

public sealed record EnterpriseConsolidationBuild(
    EnterpriseConsolidationRun Run,
    EnterpriseEliminationEntry[] Entries,
    Guid[] FarmIds,
    Guid[] CompanyIds);
