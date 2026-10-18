namespace EdgIQ.MurimiOS.Application.Eudr.AddPlot;

public sealed record AddEudrPlotRequest(Guid? Id, Guid FieldId, Guid BoundaryVersionId, Guid? HarvestLotId,
    string PlotReference, DateOnly ProductionFrom, DateOnly ProductionTo);
