using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ScotEidSubmissionResult(bool Accepted, bool Retriable, int? HttpStatus,
    string? MovementReference, string? ErrorCode, string? ErrorMessage, string ResponseJson,
    string RequestSha256);
