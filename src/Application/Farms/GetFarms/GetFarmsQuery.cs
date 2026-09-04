using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.GetFarms;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

public sealed record GetFarmsQuery() : IRequest<UseCaseResult>;
