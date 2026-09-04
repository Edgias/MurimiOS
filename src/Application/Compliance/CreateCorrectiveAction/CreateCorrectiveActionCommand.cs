using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.CreateCorrectiveAction;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;

public sealed record CreateCorrectiveActionCommand(Guid findingId, CreateCorrectiveActionRequest request) : IRequest<UseCaseResult>;
