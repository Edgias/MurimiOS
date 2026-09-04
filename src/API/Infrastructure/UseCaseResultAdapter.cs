using EdgIQ.MurimiOS.Application.Common.Results;

namespace EdgIQ.MurimiOS.API.Infrastructure;
using UseCaseResult = global::EdgIQ.MurimiOS.Application.Common.Results.UseCaseResult;
using ContentResult = global::EdgIQ.MurimiOS.Application.Common.Results.ContentResult;

public static class UseCaseResultAdapter
{
    public static IResult ToHttpResult(this UseCaseResult result) => result.Accept(Visitor.Instance);

    private sealed class Visitor : IUseCaseResultVisitor<IResult>
    {
        public static Visitor Instance { get; } = new();
        public IResult Visit<T>(ValueResult<T> result) => result.Kind switch
        {
            "Ok" => TypedResults.Ok(result.Value),
            "Created" => TypedResults.Created(result.Location, result.Value),
            "Accepted" => TypedResults.Accepted(result.Location, result.Value),
            "BadRequest" => TypedResults.BadRequest(result.Value),
            "Conflict" => TypedResults.Conflict(result.Value),
            "NotFound" => TypedResults.NotFound(result.Value),
            "UnprocessableEntity" => TypedResults.UnprocessableEntity(result.Value),
            "Json" => Results.Json(result.Value, result.JsonOptions, result.ContentType, result.Status),
            _ => throw new InvalidOperationException($"Unknown result kind: {result.Kind}")
        };

        public IResult Visit(ContentResult result) => result.Kind switch
        {
            "Ok" => TypedResults.Ok(),
            "BadRequest" => TypedResults.BadRequest(),
            "Conflict" => TypedResults.Conflict(),
            "NotFound" => TypedResults.NotFound(),
            "NoContent" => TypedResults.NoContent(),
            "Forbid" => TypedResults.Forbid(),
            "StatusCode" => TypedResults.StatusCode(result.Status!.Value),
            "Text" => Results.Text((string?)result.Content, result.ContentType, result.Encoding, result.Status),
            "File" => Results.File((byte[])result.Content!, result.ContentType, result.FileName, enableRangeProcessing: result.EnableRangeProcessing),
            "Stream" => Results.Stream((Stream)result.Content!, result.ContentType, result.FileName, enableRangeProcessing: result.EnableRangeProcessing),
            "Problem" => Results.Problem(result.Detail, result.Instance, result.Status, result.Title, result.Type, result.Extensions),
            _ => throw new InvalidOperationException($"Unknown result kind: {result.Kind}")
        };
    }
}
