using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeDerivedMediaHold;
public sealed record DerivedMediaHoldRequest(string? Reason);
