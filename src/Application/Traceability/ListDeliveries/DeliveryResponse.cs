using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListDeliveries;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;

using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
public sealed record DeliveryResponse(Guid Id, Guid FarmId, string DeliveryNumber, string Recipient, string Destination,
    DeliveryStatus Status, DateTimeOffset DispatchedAt, DateTimeOffset? ReceivedAt, string? TransactionReference);
