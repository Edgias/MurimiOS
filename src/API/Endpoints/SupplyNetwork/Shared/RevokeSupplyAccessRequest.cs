using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.Shared;
public sealed record RevokeSupplyAccessRequest(string Reason);
