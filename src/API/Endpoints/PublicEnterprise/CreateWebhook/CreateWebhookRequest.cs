using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateWebhook;
public sealed record CreateWebhookRequest(Guid? Id, string Name, string Endpoint, IReadOnlyList<string> EventTypes, int MaximumAttempts = 10);
