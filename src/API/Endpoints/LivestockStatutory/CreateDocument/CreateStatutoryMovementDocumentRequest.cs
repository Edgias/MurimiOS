using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateDocument;

public sealed record CreateStatutoryMovementDocumentRequest(Guid? Id, string? DocumentNumber, string? SchemaVersion, Guid? ProfileId = null);
