using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.Discovery;


public sealed class DiscoveryHandler(IResponseMetadata response) : IRequestHandler<DiscoveryQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DiscoveryQuery message, CancellationToken cancellationToken) => Task.FromResult(Discovery(response));

    private static UseCaseResult Discovery(IResponseMetadata response)
    {
        StandardHeaders(response);
        response["Allow"] = "GET,OPTIONS";
        response["GS1-EPC-Format"] = "Always_GS1_Digital_Link";
        return UseCaseResults.NoContent();
    }


    private static void StandardHeaders(IResponseMetadata response)
    {
        response["GS1-EPCIS-Version"] = EpcisInterop.EpcisVersion;
        response["GS1-CBV-Version"] = EpcisInterop.CbvVersion;
        response["GS1-Extensions"] = "https://murimios.io/epcis/";
    }

}
