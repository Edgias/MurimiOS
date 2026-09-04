using EdgIQ.MurimiOS.Domain.LivestockSubject;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Livestock.GetLivestockSubjects;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed record LivestockSubjectResponse(
    Guid Id,
    Guid FarmId,
    LivestockSubjectKind Kind,
    string Name,
    string Species,
    string? OfficialIdentifier);
