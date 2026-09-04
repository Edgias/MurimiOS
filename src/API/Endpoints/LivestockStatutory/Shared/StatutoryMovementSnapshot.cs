using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.Shared;
public sealed record StatutoryMovementSnapshot(int SnapshotVersion, string AnimalId, string DepartureMainCph,
    string DestinationMainCph, string DestinationLocation, DateOnly DepartureDate, DateOnly MoveDate,
    string UserReference, Guid MovementId, Guid SubjectId, Guid FarmId);
