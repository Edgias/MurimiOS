using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateDispatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed record CreateLivestockDispatchRequest(
    Guid SubjectId,
    LivestockDispatchPurpose Purpose,
    DateTimeOffset DispatchedAt,
    string Destination,
    string? OverrideReason);
