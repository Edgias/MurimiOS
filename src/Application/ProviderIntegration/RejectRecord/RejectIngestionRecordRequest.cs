using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.RejectRecord;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record RejectIngestionRecordRequest(string Code, string Reason);
