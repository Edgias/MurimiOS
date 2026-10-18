using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Eudr.AddPlot;

public sealed record AddPlotCommand(Guid CaseId, AddEudrPlotRequest Request) : IRequest<UseCaseResult>;
