using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PostRetailTokensTokenIdGuidRevoke;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

public sealed class PostRetailTokensTokenIdGuidRevokeHandler(IPlatformDbContext db) : IRequestHandler<PostRetailTokensTokenIdGuidRevokeCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostRetailTokensTokenIdGuidRevokeCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.tokenId, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid tokenId, IPlatformDbContext db, CancellationToken ct) { TransparencyToken? token = await db.TransparencyTokens.SingleOrDefaultAsync(x => x.Id == tokenId, ct); if (token is null) return UseCaseResults.NotFound(); try { token.Revoke(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(token); } catch (InvalidOperationException e) { return Conflict("transparency_token_transition_invalid", e.Message); } }
    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
