using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Compliance;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ListStandardPacks;


public sealed class ListStandardPacksHandler(ComplianceStandardPackCatalog catalog) : IRequestHandler<ListStandardPacksQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListStandardPacksQuery message, CancellationToken cancellationToken) => Task.FromResult(ListStandardPacks(catalog));

    private static UseCaseResult ListStandardPacks(ComplianceStandardPackCatalog catalog) =>UseCaseResults.Ok(catalog.Packs.Select(pack => new
    {
        pack.Key, pack.Code, pack.Name, pack.Scheme, pack.VersionLabel, pack.Revision, pack.EffectiveFrom,
        pack.Jurisdiction, pack.SourceUri, pack.Notice, pack.Policy, pack.ManifestSha256, controlCount = pack.Controls.Count
    }));

}
