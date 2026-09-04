using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class ProviderAdapterException(
    string code,
    string message,
    bool retriable,
    int? httpStatusCode = null,
    Exception? innerException = null) : Exception(message, innerException)
{
    public string Code { get; } = code;
    public bool Retriable { get; } = retriable;
    public int? HttpStatusCode { get; } = httpStatusCode;
}
