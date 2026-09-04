using System.Security.Cryptography;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class MediaOffsetMismatchException(long expectedOffset) : Exception
{
    public long ExpectedOffset { get; } = expectedOffset;
}
