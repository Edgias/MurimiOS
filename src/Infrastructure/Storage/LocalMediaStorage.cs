using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Security.Cryptography;
using Minio;
using Minio.DataModel.Args;

namespace EdgIQ.MurimiOS.Infrastructure.Storage;
using IMediaStorage = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IMediaStorage;
using MediaChunkSizeException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MediaChunkSizeException;
using MediaInspection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MediaInspection;
using MediaOffsetMismatchException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MediaOffsetMismatchException;

public interface IMediaStorageHealthProbe
{
    Task CheckHealthAsync(CancellationToken cancellationToken);
}

public sealed class LocalMediaStorage(string rootPath) : IMediaStorage, IMediaStorageHealthProbe
{
    private readonly string root = Path.GetFullPath(rootPath);

    public async Task<long> AppendChunkAsync(
        Guid tenantId,
        Guid uploadId,
        long expectedOffset,
        Stream content,
        long maximumChunkBytes,
        CancellationToken cancellationToken)
    {
        if (maximumChunkBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumChunkBytes));
        }

        string path = PendingPath(tenantId, uploadId);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var buffer = new MemoryStream();
        byte[] readBuffer = new byte[64 * 1024];
        int bytesRead;
        while ((bytesRead = await content.ReadAsync(readBuffer, cancellationToken)) > 0)
        {
            if (buffer.Length + bytesRead > maximumChunkBytes)
            {
                throw new MediaChunkSizeException(maximumChunkBytes);
            }

            await buffer.WriteAsync(readBuffer.AsMemory(0, bytesRead), cancellationToken);
        }

        if (buffer.Length == 0)
        {
            throw new MediaChunkSizeException(maximumChunkBytes);
        }

        await using var destination = new FileStream(
            path,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.WriteThrough);
        if (destination.Length != expectedOffset)
        {
            throw new MediaOffsetMismatchException(destination.Length);
        }

        destination.Position = destination.Length;
        buffer.Position = 0;
        await buffer.CopyToAsync(destination, cancellationToken);
        await destination.FlushAsync(cancellationToken);
        return destination.Length;
    }

    public async Task<MediaInspection> InspectPendingAsync(
        Guid tenantId,
        Guid uploadId,
        CancellationToken cancellationToken)
    {
        string path = PendingPath(tenantId, uploadId);
        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        byte[] header = new byte[Math.Min(12, stream.Length)];
        int headerLength = await stream.ReadAsync(header, cancellationToken);
        stream.Position = 0;
        byte[] digest = await SHA256.HashDataAsync(stream, cancellationToken);
        return new MediaInspection(
            stream.Length,
            Convert.ToHexStringLower(digest),
            header.AsSpan(0, headerLength).ToArray());
    }

    public Task<string> CommitAsync(
        Guid tenantId,
        Guid uploadId,
        Guid evidenceId,
        string contentType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string source = PendingPath(tenantId, uploadId);
        string objectKey = $"{tenantId:N}/{evidenceId:N}";
        string destination = ObjectPath(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

        if (File.Exists(destination))
        {
            if (File.Exists(source))
            {
                File.Delete(source);
            }

            return Task.FromResult(objectKey);
        }

        File.Move(source, destination);
        return Task.FromResult(objectKey);
    }

    public Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string path = ObjectPath(objectKey);
        Stream stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(stream);
    }

    public async Task<string> PutImportAsync(Guid tenantId, Guid importId, string contentType, Stream content,
        long byteSize, CancellationToken cancellationToken)
    {
        if (byteSize is <= 0 or > 52_428_800) throw new ArgumentOutOfRangeException(nameof(byteSize));
        string objectKey = $"{tenantId:N}/imports/{importId:N}";
        string destination = ObjectPath(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        if (File.Exists(destination)) return objectKey;
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            64 * 1024, FileOptions.Asynchronous | FileOptions.WriteThrough);
        await content.CopyToAsync(output, cancellationToken);
        await output.FlushAsync(cancellationToken);
        if (output.Length != byteSize)
        {
            output.Close();
            File.Delete(destination);
            throw new InvalidDataException("The stored import length does not match its declared size.");
        }
        return objectKey;
    }

    public async Task<string> PutExportAsync(Guid tenantId, Guid exportId, string category, string fileExtension,
        string contentType, Stream content, long byteSize, CancellationToken cancellationToken)
    {
        ValidateExport(category, fileExtension, byteSize);
        string objectKey = $"{tenantId:N}/exports/{category}/{exportId:N}.{fileExtension}";
        string destination = ObjectPath(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        if (File.Exists(destination)) return objectKey;
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            64 * 1024, FileOptions.Asynchronous | FileOptions.WriteThrough);
        await content.CopyToAsync(output, cancellationToken);
        await output.FlushAsync(cancellationToken);
        if (output.Length != byteSize)
        {
            output.Close(); File.Delete(destination);
            throw new InvalidDataException("The stored export length does not match its declared size.");
        }
        return objectKey;
    }

    public async Task<string> PutDerivedAsync(Guid tenantId, Guid assetId, string fileExtension, string contentType,
        Stream content, long byteSize, CancellationToken cancellationToken)
    {
        ValidateDerived(fileExtension, byteSize);
        string objectKey = $"{tenantId:N}/derived/{assetId:N}.{fileExtension.ToLowerInvariant()}";
        string destination = ObjectPath(objectKey);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        if (File.Exists(destination)) return objectKey;
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            64 * 1024, FileOptions.Asynchronous | FileOptions.WriteThrough);
        await content.CopyToAsync(output, cancellationToken);
        await output.FlushAsync(cancellationToken);
        if (output.Length != byteSize)
        {
            output.Close(); File.Delete(destination);
            throw new InvalidDataException("The stored derived-media length does not match its declared size.");
        }
        return objectKey;
    }

    public Task DeleteDerivedAsync(Guid tenantId, string objectKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string prefix = $"{tenantId:N}/derived/";
        if (!objectKey.StartsWith(prefix, StringComparison.Ordinal) || objectKey.Length <= prefix.Length)
            throw new InvalidOperationException("Only tenant-scoped derived-media objects can be deleted.");
        string path = ObjectPath(objectKey);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    public Task<Stream> OpenPendingReadAsync(
        Guid tenantId,
        Guid uploadId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Stream stream = new FileStream(
            PendingPath(tenantId, uploadId),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Task.FromResult(stream);
    }

    public Task DeletePendingAsync(Guid tenantId, Guid uploadId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string path = PendingPath(tenantId, uploadId);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    public async Task CheckHealthAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(root);
        string probe = GuardPath(Path.Combine(root, $".health-{Guid.NewGuid():N}"));
        try
        {
            await File.WriteAllTextAsync(probe, DateTimeOffset.UtcNow.ToString("O"), cancellationToken);
        }
        finally
        {
            if (File.Exists(probe)) File.Delete(probe);
        }
    }

    private string PendingPath(Guid tenantId, Guid uploadId) =>
        GuardPath(Path.Combine(root, "pending", tenantId.ToString("N"), $"{uploadId:N}.part"));

    private string ObjectPath(string objectKey) => GuardPath(Path.Combine(root, "objects", objectKey));

    internal static void ValidateExport(string category, string fileExtension, long byteSize)
    {
        if (byteSize is <= 0 or > 536_870_912) throw new ArgumentOutOfRangeException(nameof(byteSize));
        if (string.IsNullOrWhiteSpace(category) || category.Length > 80
            || category.Any(x => !char.IsAsciiLetterOrDigit(x) && x != '-'))
            throw new ArgumentException("The export category is invalid.", nameof(category));
        if (string.IsNullOrWhiteSpace(fileExtension) || fileExtension.Length > 12
            || fileExtension.Any(x => !char.IsAsciiLetterOrDigit(x)))
            throw new ArgumentException("The export extension is invalid.", nameof(fileExtension));
    }

    internal static void ValidateDerived(string fileExtension, long byteSize)
    {
        if (byteSize is <= 0 or > 2_147_483_648) throw new ArgumentOutOfRangeException(nameof(byteSize));
        if (string.IsNullOrWhiteSpace(fileExtension) || fileExtension.Length > 12
            || fileExtension.Any(x => !char.IsAsciiLetterOrDigit(x)))
            throw new ArgumentException("The derived-media extension is invalid.", nameof(fileExtension));
    }

    private string GuardPath(string candidate)
    {
        string resolved = Path.GetFullPath(candidate);
        string rootedPrefix = root.EndsWith(Path.DirectorySeparatorChar) ? root : root + Path.DirectorySeparatorChar;
        if (!resolved.StartsWith(rootedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The media object path escaped its configured root.");
        }

        return resolved;
    }
}

public sealed class S3MediaStorageOptions
{
    public string Endpoint { get; set; } = "localhost:59300";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = "murimios-media";
    public bool UseTls { get; set; }
    public bool AutoCreateBucket { get; set; }
    public string StagingRoot { get; set; } = "App_Data/media-staging";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Endpoint)) throw new InvalidOperationException("An S3 endpoint is required.");
        if (string.IsNullOrWhiteSpace(AccessKey) || string.IsNullOrWhiteSpace(SecretKey))
            throw new InvalidOperationException("S3 credentials are required.");
        if (string.IsNullOrWhiteSpace(Bucket) || Bucket.Length is < 3 or > 63)
            throw new InvalidOperationException("The S3 bucket name is invalid.");
        if (string.IsNullOrWhiteSpace(StagingRoot)) throw new InvalidOperationException("A media staging root is required.");
    }
}

public sealed class S3MediaStorage : IMediaStorage, IMediaStorageHealthProbe, IDisposable
{
    private readonly S3MediaStorageOptions options;
    private readonly LocalMediaStorage staging;
    private readonly IMinioClient client;
    private readonly SemaphoreSlim bucketLock = new(1, 1);
    private volatile bool bucketReady;

    public S3MediaStorage(S3MediaStorageOptions options)
    {
        options.Validate();
        this.options = options;
        staging = new LocalMediaStorage(options.StagingRoot);
        client = new MinioClient()
            .WithEndpoint(options.Endpoint)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .WithSSL(options.UseTls)
            .Build();
    }

    public Task<long> AppendChunkAsync(Guid tenantId, Guid uploadId, long expectedOffset, Stream content,
        long maximumChunkBytes, CancellationToken cancellationToken) =>
        staging.AppendChunkAsync(tenantId, uploadId, expectedOffset, content, maximumChunkBytes, cancellationToken);

    public Task<MediaInspection> InspectPendingAsync(Guid tenantId, Guid uploadId, CancellationToken cancellationToken) =>
        staging.InspectPendingAsync(tenantId, uploadId, cancellationToken);

    public Task<Stream> OpenPendingReadAsync(Guid tenantId, Guid uploadId, CancellationToken cancellationToken) =>
        staging.OpenPendingReadAsync(tenantId, uploadId, cancellationToken);

    public Task DeletePendingAsync(Guid tenantId, Guid uploadId, CancellationToken cancellationToken) =>
        staging.DeletePendingAsync(tenantId, uploadId, cancellationToken);

    public async Task<string> CommitAsync(
        Guid tenantId,
        Guid uploadId,
        Guid evidenceId,
        string contentType,
        CancellationToken cancellationToken)
    {
        await EnsureBucketAsync(cancellationToken);
        string objectKey = $"tenants/{tenantId:N}/evidence/{evidenceId:N}";
        await using (Stream source = await staging.OpenPendingReadAsync(tenantId, uploadId, cancellationToken))
        {
            var args = new PutObjectArgs()
                .WithBucket(options.Bucket)
                .WithObject(objectKey)
                .WithStreamData(source)
                .WithObjectSize(source.Length)
                .WithContentType(contentType);
            await client.PutObjectAsync(args, cancellationToken);
        }
        await staging.DeletePendingAsync(tenantId, uploadId, cancellationToken);
        return objectKey;
    }

    public async Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken)
    {
        await EnsureBucketAsync(cancellationToken);
        var destination = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(options.Bucket)
            .WithObject(objectKey)
            .WithCallbackStream(async (source, token) => await source.CopyToAsync(destination, token));
        await client.GetObjectAsync(args, cancellationToken);
        destination.Position = 0;
        return destination;
    }

    public async Task<string> PutImportAsync(Guid tenantId, Guid importId, string contentType, Stream content,
        long byteSize, CancellationToken cancellationToken)
    {
        if (byteSize is <= 0 or > 52_428_800) throw new ArgumentOutOfRangeException(nameof(byteSize));
        await EnsureBucketAsync(cancellationToken);
        string objectKey = $"tenants/{tenantId:N}/imports/{importId:N}";
        var args = new PutObjectArgs()
            .WithBucket(options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(byteSize)
            .WithContentType(contentType);
        await client.PutObjectAsync(args, cancellationToken);
        return objectKey;
    }

    public async Task<string> PutExportAsync(Guid tenantId, Guid exportId, string category, string fileExtension,
        string contentType, Stream content, long byteSize, CancellationToken cancellationToken)
    {
        LocalMediaStorage.ValidateExport(category, fileExtension, byteSize);
        await EnsureBucketAsync(cancellationToken);
        string objectKey = $"tenants/{tenantId:N}/exports/{category}/{exportId:N}.{fileExtension}";
        var args = new PutObjectArgs()
            .WithBucket(options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(byteSize)
            .WithContentType(contentType);
        await client.PutObjectAsync(args, cancellationToken);
        return objectKey;
    }

    public async Task<string> PutDerivedAsync(Guid tenantId, Guid assetId, string fileExtension, string contentType,
        Stream content, long byteSize, CancellationToken cancellationToken)
    {
        LocalMediaStorage.ValidateDerived(fileExtension, byteSize);
        await EnsureBucketAsync(cancellationToken);
        string objectKey = $"tenants/{tenantId:N}/derived/{assetId:N}.{fileExtension.ToLowerInvariant()}";
        var args = new PutObjectArgs().WithBucket(options.Bucket).WithObject(objectKey).WithStreamData(content)
            .WithObjectSize(byteSize).WithContentType(contentType);
        await client.PutObjectAsync(args, cancellationToken);
        return objectKey;
    }

    public async Task DeleteDerivedAsync(Guid tenantId, string objectKey, CancellationToken cancellationToken)
    {
        string prefix = $"tenants/{tenantId:N}/derived/";
        if (!objectKey.StartsWith(prefix, StringComparison.Ordinal) || objectKey.Length <= prefix.Length)
            throw new InvalidOperationException("Only tenant-scoped derived-media objects can be deleted.");
        await EnsureBucketAsync(cancellationToken);
        await client.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(options.Bucket).WithObject(objectKey),
            cancellationToken);
    }

    public async Task CheckHealthAsync(CancellationToken cancellationToken)
    {
        bool exists = await client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(options.Bucket), cancellationToken);
        if (!exists && !options.AutoCreateBucket)
            throw new InvalidOperationException($"S3 bucket '{options.Bucket}' does not exist.");
        if (!exists) await EnsureBucketAsync(cancellationToken);
    }

    private async Task EnsureBucketAsync(CancellationToken cancellationToken)
    {
        if (bucketReady) return;
        await bucketLock.WaitAsync(cancellationToken);
        try
        {
            if (bucketReady) return;
            bool exists = await client.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(options.Bucket), cancellationToken);
            if (!exists)
            {
                if (!options.AutoCreateBucket)
                    throw new InvalidOperationException($"S3 bucket '{options.Bucket}' does not exist.");
                await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(options.Bucket), cancellationToken);
            }
            bucketReady = true;
        }
        finally
        {
            bucketLock.Release();
        }
    }

    public void Dispose()
    {
        client.Dispose();
        bucketLock.Dispose();
    }
}
