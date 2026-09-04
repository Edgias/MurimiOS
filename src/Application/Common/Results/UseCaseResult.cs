using System.Text;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Results;

// Keeps the original payload's declared type without making Application depend on
// ASP.NET result types. The host supplies the visitor that writes the response.
public abstract record UseCaseResult
{
    public abstract TResult Accept<TResult>(IUseCaseResultVisitor<TResult> visitor);
}

public interface IUseCaseResultVisitor<out TResult>
{
    TResult Visit<T>(ValueResult<T> result);
    TResult Visit(ContentResult result);
}

public sealed record ValueResult<T>(string Kind, T? Value, string? Location = null,
    int? Status = null, JsonSerializerOptions? JsonOptions = null, string? ContentType = null) : UseCaseResult
{
    public override TResult Accept<TResult>(IUseCaseResultVisitor<TResult> visitor) => visitor.Visit(this);
}

public sealed record ContentResult(string Kind, object? Content = null, string? ContentType = null,
    string? FileName = null, Encoding? Encoding = null, int? Status = null,
    bool EnableRangeProcessing = false, string? Detail = null, string? Title = null,
    string? Type = null, string? Instance = null, IDictionary<string, object?>? Extensions = null) : UseCaseResult
{
    public override TResult Accept<TResult>(IUseCaseResultVisitor<TResult> visitor) => visitor.Visit(this);
}

public static class UseCaseResults
{
    public static UseCaseResult Ok<T>(T? value) => new ValueResult<T>("Ok", value);
    public static UseCaseResult Ok() => new ContentResult("Ok");
    public static UseCaseResult Created<T>(string? uri, T? value) => new ValueResult<T>("Created", value, uri);
    public static UseCaseResult Accepted<T>(string? uri = null, T? value = default) => new ValueResult<T>("Accepted", value, uri);
    public static UseCaseResult BadRequest<T>(T? error) => new ValueResult<T>("BadRequest", error);
    public static UseCaseResult BadRequest() => new ContentResult("BadRequest");
    public static UseCaseResult Conflict<T>(T? error) => new ValueResult<T>("Conflict", error);
    public static UseCaseResult Conflict() => new ContentResult("Conflict");
    public static UseCaseResult NotFound<T>(T? value) => new ValueResult<T>("NotFound", value);
    public static UseCaseResult NotFound() => new ContentResult("NotFound");
    public static UseCaseResult NoContent() => new ContentResult("NoContent");
    public static UseCaseResult Forbid() => new ContentResult("Forbid");
    public static UseCaseResult StatusCode(int statusCode) => new ContentResult("StatusCode", Status: statusCode);
    public static UseCaseResult UnprocessableEntity<T>(T? error) => new ValueResult<T>("UnprocessableEntity", error);
    public static UseCaseResult Json<T>(T? data, JsonSerializerOptions? options = null, string? contentType = null, int? statusCode = null)
        => new ValueResult<T>("Json", data, Status: statusCode, JsonOptions: options, ContentType: contentType);
    public static UseCaseResult Text(string? content, string? contentType = null, Encoding? contentEncoding = null, int? statusCode = null)
        => new ContentResult("Text", content, contentType, Encoding: contentEncoding, Status: statusCode);
    public static UseCaseResult File(byte[] fileContents, string? contentType = null, string? fileDownloadName = null, bool enableRangeProcessing = false)
        => new ContentResult("File", fileContents, contentType, fileDownloadName, EnableRangeProcessing: enableRangeProcessing);
    public static UseCaseResult File(Stream stream, string? contentType = null, string? fileDownloadName = null, bool enableRangeProcessing = false)
        => new ContentResult("Stream", stream, contentType, fileDownloadName, EnableRangeProcessing: enableRangeProcessing);
    public static UseCaseResult Stream(Stream stream, string? contentType = null, string? fileDownloadName = null, bool enableRangeProcessing = false)
        => new ContentResult("Stream", stream, contentType, fileDownloadName, EnableRangeProcessing: enableRangeProcessing);
    public static UseCaseResult Problem(string? detail = null, string? instance = null, int? statusCode = null, string? title = null, string? type = null, IDictionary<string, object?>? extensions = null)
        => new ContentResult("Problem", Status: statusCode, Detail: detail, Title: title, Type: type, Instance: instance, Extensions: extensions);
}
