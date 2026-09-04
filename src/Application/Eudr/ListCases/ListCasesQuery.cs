using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.ListCases;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;

public sealed record ListCasesQuery(Guid? farmId, EudrCaseStatus? status) : IRequest<UseCaseResult>;
