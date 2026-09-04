using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateTenure;
using LandTenureType = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureType;

public sealed record CreateLandTenureRequest(Guid? Id, Guid? FieldId, LandTenureType Type, string HolderName,
    string? CounterpartyName, string Reference, decimal? RegisteredAreaHa, DateOnly ValidFrom, DateOnly? ValidTo,
    string? DocumentReference, string? Notes);
