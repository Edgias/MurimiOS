using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface IComplianceAuditPackSignatureService
{
    string KeyId { get; }

    string Sign(byte[] content);
    bool Verify(string keyId, byte[] content, string signatureBase64);
}
