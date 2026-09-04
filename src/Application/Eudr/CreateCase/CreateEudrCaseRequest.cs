using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.CreateCase;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCommodity = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCommodity;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record CreateEudrCaseRequest(Guid? Id, string CaseNumber, EudrCommodity Commodity,
    string ProductDescription, string HsCode, decimal Quantity, string Unit, string ProductionCountry,
    DateOnly ProductionFrom, DateOnly ProductionTo, string OperatorName, string OperatorRegistrationNumber,
    string OperatorAddress, string Market);
