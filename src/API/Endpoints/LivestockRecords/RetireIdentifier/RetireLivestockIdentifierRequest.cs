using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.RetireIdentifier;
public sealed record RetireLivestockIdentifierRequest(DateTimeOffset ValidTo);
