using System.Security.Cryptography;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record MediaInspection(long Length, string Sha256, byte[] Header);
