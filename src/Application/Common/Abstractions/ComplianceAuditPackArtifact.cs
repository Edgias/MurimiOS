using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ComplianceAuditPackArtifact(string ObjectKey, byte[] Bytes, string ArtifactSha256,
    string ManifestJson, string ManifestSha256, string SignatureBase64, string SignatureKeyId,
    int ContentEntryCount, int EvidenceFileCount);
