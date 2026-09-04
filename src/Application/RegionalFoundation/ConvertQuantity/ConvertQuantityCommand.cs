using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.ConvertQuantity;
using UnitDimension = global::EdgIQ.MurimiOS.Domain.Common.UnitDimension;
using MeasurementUnitDefinition = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnitDefinition;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

public sealed record ConvertQuantityCommand(ConvertMeasurementRequest request) : IRequest<UseCaseResult>;
