using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeTaskFile;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;

public sealed class ChangeTaskFileHandler(IPlatformDbContext db) : IRequestHandler<ChangeTaskFileCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeTaskFileCommand message, CancellationToken cancellationToken) => ChangeTaskFileAsync(message.taskFileId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeTaskFileAsync(Guid taskFileId, string command, ExchangeCommandRequest request, IPlatformDbContext db, CancellationToken ct)
    { MachineryTaskFile? file = await db.MachineryTaskFiles.SingleOrDefaultAsync(x => x.Id == taskFileId, ct); if (file is null) return UseCaseResults.NotFound(); try { if (command == "validate") file.Validate(); else if (command == "complete") file.Complete(DateTimeOffset.UtcNow); else file.Reject(DateTimeOffset.UtcNow, request.Notes ?? "Rejected"); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(file); } catch (InvalidOperationException e) { return Conflict("task_file_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
