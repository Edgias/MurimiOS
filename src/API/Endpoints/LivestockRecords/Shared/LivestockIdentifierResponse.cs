using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.Shared;
public sealed record LivestockIdentifierResponse(Guid Id, Guid SubjectId, string Namespace, string Value, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo, bool IsPrimary);
