using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateRelationship;
using SupplyPartyRole = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyPartyRole;

public sealed record CreateSupplyRelationshipRequest(Guid? Id, Guid CounterpartyTenantId, SupplyPartyRole OurRole, string Name);
