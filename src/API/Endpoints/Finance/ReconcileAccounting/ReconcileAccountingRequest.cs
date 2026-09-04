using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ReconcileAccounting;
public sealed record ReconcileAccountingRequest(decimal AmountTolerance, int DateWindowDays);
