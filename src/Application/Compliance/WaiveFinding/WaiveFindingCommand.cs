using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.WaiveFinding;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;

public sealed record WaiveFindingCommand(Guid findingId, CorrectiveActionCommandRequest request) : IRequest<UseCaseResult>;
