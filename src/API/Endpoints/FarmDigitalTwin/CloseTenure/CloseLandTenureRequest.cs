using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CloseTenure;
using LandTenureStatus = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureStatus;
public sealed record CloseLandTenureRequest(DateOnly ValidTo, LandTenureStatus Outcome);
