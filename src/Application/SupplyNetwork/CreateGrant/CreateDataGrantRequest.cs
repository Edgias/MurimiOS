using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.CreateGrant;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;
public sealed record CreateDataGrantRequest(Guid? Id, Guid RelationshipId, string Name, string Purpose,
    IReadOnlyList<string> EntityCategories, IReadOnlyList<string> Attributes, IReadOnlyList<Guid> FarmIds,
    IReadOnlyList<Guid> SeasonIds, IReadOnlyList<string> Commodities, DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo, bool DownstreamDisclosureAllowed, Guid? PreviousGrantId);
