using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.Shared;
public sealed record CorrectiveActionCommandRequest(string? Notes);
