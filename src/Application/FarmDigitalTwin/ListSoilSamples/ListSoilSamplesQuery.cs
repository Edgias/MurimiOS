using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SoilSample;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListSoilSamples;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilSampleStatus = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleStatus;

public sealed record ListSoilSamplesQuery(Guid farmId, Guid? fieldId, SoilSampleStatus? status) : IRequest<UseCaseResult>;
