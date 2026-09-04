using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListDeliveries;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;

using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;

public sealed record ListDeliveriesQuery(Guid farmId) : IRequest<UseCaseResult>;
