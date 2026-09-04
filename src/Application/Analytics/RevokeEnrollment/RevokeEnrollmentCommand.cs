using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.RevokeEnrollment;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;

public sealed record RevokeEnrollmentCommand(Guid enrollmentId) : IRequest<UseCaseResult>;
