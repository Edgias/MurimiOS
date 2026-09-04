using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.Shared;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;
public sealed record StatutorySubmissionAttemptResponse(Guid Id, int AttemptNumber, StatutorySubmissionOutcome Outcome,
    DateTimeOffset StartedAt, DateTimeOffset CompletedAt, int? HttpStatus, string? ExternalReference,
    string? ErrorCode, string? ErrorMessage);
