using System.Security.Cryptography;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class MediaChunkSizeException(long maximumChunkBytes) : Exception
{
    public long MaximumChunkBytes { get; } = maximumChunkBytes;
}
