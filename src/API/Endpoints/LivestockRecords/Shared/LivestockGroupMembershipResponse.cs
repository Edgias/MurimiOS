using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.Shared;
public sealed record LivestockGroupMembershipResponse(Guid Id, Guid GroupId, Guid MemberSubjectId, DateTimeOffset JoinedAt, DateTimeOffset? LeftAt);
