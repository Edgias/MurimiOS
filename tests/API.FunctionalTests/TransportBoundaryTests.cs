using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Application.Common.Results;
using EdgIQ.MurimiOS.Application.Common.TextEncoding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using ResponseMetadata = global::EdgIQ.MurimiOS.API.Infrastructure.ResponseMetadata;
using IncomingContentAdapter = global::EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter;
using UseCaseResults = global::EdgIQ.MurimiOS.Application.Common.Results.UseCaseResults;
using Base64UrlEncoding = global::EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding;

public sealed class TransportBoundaryTests
{
    [Fact]
    public void Base64_url_encoding_matches_the_previous_transport_implementation()
    {
        for (int length = 0; length <= 256; length++)
        {
            byte[] bytes = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
            Assert.Equal(WebEncoders.Base64UrlEncode(bytes), Base64UrlEncoding.Encode(bytes));
        }
    }

    [Fact]
    public void Request_content_is_read_lazily_and_response_headers_are_written_immediately()
    {
        var context = new DefaultHttpContext();
        var content = IncomingContentAdapter.Bind(context.Request);
        using var stream = new MemoryStream([1, 2, 3]);
        context.Request.Body = stream;
        context.Request.ContentType = "application/octet-stream";
        context.Request.ContentLength = 3;
        context.Request.Headers["Upload-Offset"] = "5";
        context.Request.QueryString = new QueryString("?fileName=sample.bin");
        Assert.Same(stream, content.Body);
        Assert.Equal(3, content.ContentLength);
        Assert.Equal("application/octet-stream", content.ContentType);
        Assert.Equal("5", content.FirstHeader("Upload-Offset"));
        Assert.Equal("sample.bin", content.FirstQuery("fileName"));
        var metadata = new ResponseMetadata(new HttpContextAccessor { HttpContext = context });
        metadata["Upload-Offset"] = "8";
        Assert.Equal("8", context.Response.Headers["Upload-Offset"].ToString());
    }

    [Fact]
    public void Problem_extensions_and_file_stream_options_survive_the_boundary()
    {
        var problem = Assert.IsType<ProblemHttpResult>(UseCaseResults.Problem(
            statusCode: 400, title: "Rejected", detail: "Reason",
            extensions: new Dictionary<string, object?> { ["code"] = "invalid" }).ToHttpResult());
        Assert.Equal(400, problem.StatusCode);
        Assert.Equal("invalid", problem.ProblemDetails.Extensions["code"]);
        using var stream = new MemoryStream([1, 2, 3]);
        var file = Assert.IsType<FileStreamHttpResult>(UseCaseResults.File(
            stream, "application/pdf", "report.pdf", enableRangeProcessing: true).ToHttpResult());
        Assert.Same(stream, file.FileStream);
        Assert.True(file.EnableRangeProcessing);
        Assert.Equal("report.pdf", file.FileDownloadName);
    }
}
