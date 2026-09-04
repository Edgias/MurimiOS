using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.ReceiveDelivery;
public sealed record ReceiveDeliveryRequest(DateTimeOffset ReceivedAt);
