using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMembership;
public sealed record CreateLivestockGroupMembershipRequest(Guid? Id, Guid MemberSubjectId, DateTimeOffset JoinedAt);
