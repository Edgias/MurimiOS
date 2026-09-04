using System.Data;
using System.Text.Json;

using EdgIQ.MurimiOS.Domain.Delivery;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.Shared;
using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
public sealed record DeliveryResponse(Guid Id, Guid FarmId, string DeliveryNumber, string Recipient, string Destination,
    DeliveryStatus Status, DateTimeOffset DispatchedAt, DateTimeOffset? ReceivedAt, string? TransactionReference);
