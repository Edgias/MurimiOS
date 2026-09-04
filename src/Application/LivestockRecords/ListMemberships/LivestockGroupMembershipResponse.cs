using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListMemberships;

public sealed record LivestockGroupMembershipResponse(Guid Id, Guid GroupId, Guid MemberSubjectId, DateTimeOffset JoinedAt, DateTimeOffset? LeftAt);
