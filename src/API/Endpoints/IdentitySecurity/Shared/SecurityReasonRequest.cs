using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.Shared;

public sealed record SecurityReasonRequest(string Reason);
