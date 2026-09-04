using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateIdentifier;
public sealed record CreateLivestockIdentifierRequest(Guid? Id, string Namespace, string Value, DateTimeOffset ValidFrom, bool IsPrimary);
