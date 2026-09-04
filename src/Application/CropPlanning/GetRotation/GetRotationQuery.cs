using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.GetRotation;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;

public sealed record GetRotationQuery(Guid fieldId) : IRequest<UseCaseResult>;
