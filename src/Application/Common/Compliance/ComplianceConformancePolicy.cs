using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
public sealed record ComplianceConformancePolicy(decimal MinimumScorePercent, bool CriticalControlsMustConform);
