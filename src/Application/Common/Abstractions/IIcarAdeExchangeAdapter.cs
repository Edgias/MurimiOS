using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface IIcarAdeExchangeAdapter
{
    Task<IcarAdeExchangeResult> SubmitAsync(IcarAdeOptions options, IcarAdeCredentials credentials, Guid submissionId, string payloadJson, CancellationToken cancellationToken);
}
