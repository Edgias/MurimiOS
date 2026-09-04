using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed record TransformationInput(TraceObjectType ObjectType, Guid ObjectId, decimal Quantity, string Unit);
