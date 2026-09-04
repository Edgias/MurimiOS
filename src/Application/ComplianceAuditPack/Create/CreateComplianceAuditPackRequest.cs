using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
namespace EdgIQ.MurimiOS.Application.ComplianceAuditPack.Create;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;
using ComplianceAuditPackFormat = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackFormat;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record CreateComplianceAuditPackRequest(Guid? Id, string ScopeStatement, bool IncludeEvidenceFiles = true);
