using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.AddPlot;
public sealed record AddEudrPlotRequest(Guid? Id, Guid FieldId, Guid BoundaryVersionId, Guid? HarvestLotId,
    string PlotReference, DateOnly ProductionFrom, DateOnly ProductionTo);
