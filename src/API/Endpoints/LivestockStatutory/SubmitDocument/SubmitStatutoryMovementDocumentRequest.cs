using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.SubmitDocument;
public sealed record SubmitStatutoryMovementDocumentRequest(Guid ConnectionId);
