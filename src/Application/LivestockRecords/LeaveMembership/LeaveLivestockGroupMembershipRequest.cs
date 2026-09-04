using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.LeaveMembership;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;
public sealed record LeaveLivestockGroupMembershipRequest(DateTimeOffset LeftAt);
