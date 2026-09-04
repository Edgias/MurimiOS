using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.GetDispatchEligibility;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;

public sealed record GetDispatchEligibilityQuery(Guid subjectId, LivestockDispatchPurpose purpose, DateTimeOffset? at) : IRequest<UseCaseResult>;
