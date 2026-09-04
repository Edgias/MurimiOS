using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateCorrectiveAction;
public sealed record CreateCorrectiveActionRequest(Guid? Id, string Action, string Owner, DateOnly DueOn);
