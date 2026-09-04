using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ScotEidOptions(Uri Endpoint, string ApplicationName, string ApplicationVersion,
    string SchemaVersion, string SoapAction = "SMECreateCattleMovements");
