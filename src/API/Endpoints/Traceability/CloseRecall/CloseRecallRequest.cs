using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CloseRecall;
public sealed record CloseRecallRequest(DateTimeOffset ClosedAt);
