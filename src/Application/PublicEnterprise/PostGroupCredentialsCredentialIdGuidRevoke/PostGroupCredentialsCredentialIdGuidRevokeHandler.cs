using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PostGroupCredentialsCredentialIdGuidRevoke;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;

public sealed class PostGroupCredentialsCredentialIdGuidRevokeHandler(IPlatformDbContext db) : IRequestHandler<PostGroupCredentialsCredentialIdGuidRevokeCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostGroupCredentialsCredentialIdGuidRevokeCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.credentialId, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid credentialId, IPlatformDbContext db, CancellationToken ct) { PublicApiCredential? credential = await db.PublicApiCredentials.SingleOrDefaultAsync(x => x.Id == credentialId, ct); if (credential is null) return UseCaseResults.NotFound(); try { credential.Revoke(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(credential); } catch (InvalidOperationException e) { return Conflict("api_credential_transition_invalid", e.Message); } }
    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
