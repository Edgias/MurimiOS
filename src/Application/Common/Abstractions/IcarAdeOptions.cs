using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public sealed record IcarAdeOptions(Uri Endpoint, string OrganisationId, string SchemaVersion);
