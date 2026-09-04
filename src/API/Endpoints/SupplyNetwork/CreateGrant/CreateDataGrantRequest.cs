using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateGrant;
public sealed record CreateDataGrantRequest(Guid? Id, Guid RelationshipId, string Name, string Purpose,
    IReadOnlyList<string> EntityCategories, IReadOnlyList<string> Attributes, IReadOnlyList<Guid> FarmIds,
    IReadOnlyList<Guid> SeasonIds, IReadOnlyList<string> Commodities, DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo, bool DownstreamDisclosureAllowed, Guid? PreviousGrantId);
