using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateDelivery;
public sealed record CreateDeliveryRequest(Guid? Id, string DeliveryNumber, string Recipient, string Destination,
    DateTimeOffset DispatchedAt, string? TransactionReference, IReadOnlyList<DeliverySourceLine> Lines);
