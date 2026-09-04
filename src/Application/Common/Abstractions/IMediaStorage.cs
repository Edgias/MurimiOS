using System.Security.Cryptography;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface IMediaStorage
{
    Task<long> AppendChunkAsync(
        Guid tenantId,
        Guid uploadId,
        long expectedOffset,
        Stream content,
        long maximumChunkBytes,
        CancellationToken cancellationToken);

    Task<MediaInspection> InspectPendingAsync(
        Guid tenantId,
        Guid uploadId,
        CancellationToken cancellationToken);

    Task<Stream> OpenPendingReadAsync(
        Guid tenantId,
        Guid uploadId,
        CancellationToken cancellationToken);

    Task<string> CommitAsync(
        Guid tenantId,
        Guid uploadId,
        Guid evidenceId,
        string contentType,
        CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken);

    Task<string> PutImportAsync(
        Guid tenantId,
        Guid importId,
        string contentType,
        Stream content,
        long byteSize,
        CancellationToken cancellationToken);

    Task<string> PutExportAsync(
        Guid tenantId,
        Guid exportId,
        string category,
        string fileExtension,
        string contentType,
        Stream content,
        long byteSize,
        CancellationToken cancellationToken);

    Task<string> PutDerivedAsync(
        Guid tenantId,
        Guid assetId,
        string fileExtension,
        string contentType,
        Stream content,
        long byteSize,
        CancellationToken cancellationToken);

    Task DeleteDerivedAsync(Guid tenantId, string objectKey, CancellationToken cancellationToken);

    Task DeletePendingAsync(Guid tenantId, Guid uploadId, CancellationToken cancellationToken);
}
