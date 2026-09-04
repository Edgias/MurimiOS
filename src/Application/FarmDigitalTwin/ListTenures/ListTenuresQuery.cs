using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListTenures;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;

public sealed record ListTenuresQuery(Guid farmId, Guid? fieldId) : IRequest<UseCaseResult>;
