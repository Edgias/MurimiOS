using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.CreateRelationship;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyPartyRole = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyPartyRole;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed record CreateSupplyRelationshipRequest(Guid? Id, Guid CounterpartyTenantId, SupplyPartyRole OurRole, string Name);
