using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface IScotEidMovementAdapter
{
    Task<ScotEidSubmissionResult> SubmitAsync(ScotEidOptions options, ScotEidCredentials credentials, ScotEidMovementSubmission movement, CancellationToken cancellationToken);
}
