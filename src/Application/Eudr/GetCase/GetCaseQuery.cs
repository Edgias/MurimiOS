using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.GetCase;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;

public sealed record GetCaseQuery(Guid caseId) : IRequest<UseCaseResult>;
