using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListIdentifiers;

public sealed record LivestockIdentifierResponse(Guid Id, Guid SubjectId, string Namespace, string Value, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo, bool IsPrimary);
