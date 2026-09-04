using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Shared;
public sealed record CreateLivestockExchangeSubmissionRequest(Guid? Id, Guid? ProfileId);
