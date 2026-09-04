using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Acknowledge;
public sealed record AcknowledgeLivestockExchangeRequest(bool Accepted, string? ExternalReference,
    string? ErrorCode, string? ErrorMessage, string? ResponseJson);
