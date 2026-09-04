using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ScotEidCredentials(string Username, string Password, string ApplicationKey);
