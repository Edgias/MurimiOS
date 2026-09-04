using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed record EvidenceRow(
    Guid Id,
    DateTimeOffset CapturedAt,
    string FileName,
    string ContentType,
    long ByteSize,
    double? Latitude,
    double? Longitude,
    double? AccuracyMetres,
    string Sha256);
