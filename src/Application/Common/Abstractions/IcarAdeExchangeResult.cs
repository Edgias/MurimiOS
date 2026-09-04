using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record IcarAdeExchangeResult(bool Accepted, bool Pending, bool Retriable, int? HttpStatus,
    string? ExternalReference, string? ErrorCode, string? ErrorMessage, string ResponseJson,
    string RequestSha256);
