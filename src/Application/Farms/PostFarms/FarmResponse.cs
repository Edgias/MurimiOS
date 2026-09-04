using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.PostFarms;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;

public sealed record FarmResponse(
    Guid Id,
    Guid OrganisationNodeId,
    string Name,
    string Code,
    string CountryCode,
    string TimeZone);
