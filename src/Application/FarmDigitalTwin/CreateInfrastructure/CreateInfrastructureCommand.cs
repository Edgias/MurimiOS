using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateInfrastructure;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
using InfrastructureAssetType = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetType;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CreateInfrastructureCommand(Guid farmId, CreateInfrastructureAssetRequest request) : IRequest<UseCaseResult>;
