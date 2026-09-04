using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.Mappings;


public sealed class MappingsHandler() : IRequestHandler<MappingsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(MappingsQuery message, CancellationToken cancellationToken) => Task.FromResult(Mappings());

    private static UseCaseResult Mappings() =>UseCaseResults.Ok(new
    {
        epcisVersion = EpcisInterop.EpcisVersion, cbvVersion = EpcisInterop.CbvVersion,
        context = EpcisInterop.ContextUri,
        businessSteps = new Dictionary<string, string>
        {
            ["harvesting"] = "commissioning", ["crop-to-feed"] = "creating_class_instance",
            ["packing"] = "packing", ["shipping"] = "shipping", ["receiving"] = "receiving"
        },
        dispositions = new Dictionary<string, string>
        {
            ["harvested"] = "active", ["feed-available"] = "available", ["received"] = "available",
            ["active"] = "active", ["in-transit"] = "in_transit"
        },
        units = new Dictionary<string, string> { ["kg"] = "KGM", ["L"] = "LTR", ["each"] = "C62", ["head"] = "C62" }
    });

}
