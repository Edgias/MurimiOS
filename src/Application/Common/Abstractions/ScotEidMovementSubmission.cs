using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ScotEidMovementSubmission(string AnimalId, string DepartureMainCph,
    string DestinationMainCph, string DestinationLocation, DateOnly DepartureDate, DateOnly MoveDate,
    string UserReference);
