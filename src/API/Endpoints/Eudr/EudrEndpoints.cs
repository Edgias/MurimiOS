using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.Eudr;
using AddPlotEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.AddPlot.AddPlotEndpoint;
using AssembleStatementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.AssembleStatement.AssembleStatementEndpoint;
using CancelCaseEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.CancelCase.CancelCaseEndpoint;
using CreateCaseEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.CreateCase.CreateCaseEndpoint;
using ExportStatementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.ExportStatement.ExportStatementEndpoint;
using GetCaseEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.GetCase.GetCaseEndpoint;
using GetStatementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.GetStatement.GetStatementEndpoint;
using ListCasesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.ListCases.ListCasesEndpoint;
using RecordRiskDecisionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordRiskDecision.RecordRiskDecisionEndpoint;
using RecordScreeningEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordScreening.RecordScreeningEndpoint;
using SubmitStatementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.SubmitStatement.SubmitStatementEndpoint;


public static class EudrEndpoints
{

    public static void MapEudrEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder eudr = api.MapGroup("/eudr").WithTags("EUDR");
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.ListCases.ListCasesEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.CreateCase.CreateCaseEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.GetCase.GetCaseEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.AddPlot.AddPlotEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordScreening.RecordScreeningEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordRiskDecision.RecordRiskDecisionEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.AssembleStatement.AssembleStatementEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.GetStatement.GetStatementEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.ExportStatement.ExportStatementEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.SubmitStatement.SubmitStatementEndpoint.Map(eudr);
        global::EdgIQ.MurimiOS.API.Endpoints.Eudr.CancelCase.CancelCaseEndpoint.Map(eudr);
    }
}
