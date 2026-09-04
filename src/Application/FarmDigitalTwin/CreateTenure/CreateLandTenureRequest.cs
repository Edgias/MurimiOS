using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateTenure;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;
using LandTenureType = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CreateLandTenureRequest(Guid? Id, Guid? FieldId, LandTenureType Type, string HolderName,
    string? CounterpartyName, string Reference, decimal? RegisteredAreaHa, DateOnly ValidFrom, DateOnly? ValidTo,
    string? DocumentReference, string? Notes);
