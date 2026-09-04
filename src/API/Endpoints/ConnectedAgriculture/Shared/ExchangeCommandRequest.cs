using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.Shared;
public sealed record ExchangeCommandRequest(string? Notes, Guid? AppliedEntityId = null);
