using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.GetDocumentArtifact;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;

public sealed class GetDocumentArtifactHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetDocumentArtifactQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetDocumentArtifactQuery message, CancellationToken cancellationToken) => GetDocumentArtifactAsync(message.documentId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetDocumentArtifactAsync(Guid documentId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockStatutoryMovementDocument? document = await db.LivestockStatutoryMovementDocuments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == documentId, cancellationToken);
        if (document is null || !await scope.CanAccessFarmAsync(document.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        string extension = document.DocumentMediaType.Contains("xml", StringComparison.OrdinalIgnoreCase) ? "xml" : "json";
        return UseCaseResults.File(Encoding.UTF8.GetBytes(document.RenderedDocument), document.DocumentMediaType,
            $"{document.DocumentNumber}.{extension}");
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
