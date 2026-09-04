using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create;

public sealed record CreateComplianceAuditPackRequest(Guid? Id, string ScopeStatement, bool IncludeEvidenceFiles = true);
