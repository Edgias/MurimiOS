using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords;
public sealed record ReceiveIngestionRecordsResponse(int Accepted, int Duplicates);
