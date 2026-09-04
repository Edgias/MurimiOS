using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability;
using CloseRecallEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CloseRecall.CloseRecallEndpoint;
using CreateDeliveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateDelivery.CreateDeliveryEndpoint;
using CreateQualityAssessmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateQualityAssessment.CreateQualityAssessmentEndpoint;
using CreateRecallEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateRecall.CreateRecallEndpoint;
using CreateTransformationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation.CreateTransformationEndpoint;
using GetLineageEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetLineage.GetLineageEndpoint;
using GetMassBalanceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetMassBalance.GetMassBalanceEndpoint;
using GetRecallImpactEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetRecallImpact.GetRecallImpactEndpoint;
using ListDeliveriesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListDeliveries.ListDeliveriesEndpoint;
using ListQualityAssessmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListQualityAssessments.ListQualityAssessmentsEndpoint;
using ListRecallsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListRecalls.ListRecallsEndpoint;
using ReceiveDeliveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ReceiveDelivery.ReceiveDeliveryEndpoint;


public static class TraceabilityEndpoints
{
    public static void MapTraceabilityEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder trace = api.MapGroup("/traceability").WithTags("Traceability");
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListEvents.ListEventsEndpoint.Map(trace);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation.CreateTransformationEndpoint.Map(trace);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetLineage.GetLineageEndpoint.Map(trace);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetMassBalance.GetMassBalanceEndpoint.Map(trace);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateQualityAssessment.CreateQualityAssessmentEndpoint.Map(trace);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListQualityAssessments.ListQualityAssessmentsEndpoint.Map(trace);

        RouteGroupBuilder deliveries = api.MapGroup("/deliveries").WithTags("Traceability");
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListDeliveries.ListDeliveriesEndpoint.Map(deliveries);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateDelivery.CreateDeliveryEndpoint.Map(deliveries);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ReceiveDelivery.ReceiveDeliveryEndpoint.Map(deliveries);

        RouteGroupBuilder recalls = api.MapGroup("/recalls").WithTags("Traceability");
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.ListRecalls.ListRecallsEndpoint.Map(recalls);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateRecall.CreateRecallEndpoint.Map(recalls);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.GetRecallImpact.GetRecallImpactEndpoint.Map(recalls);
        global::EdgIQ.MurimiOS.API.Endpoints.Traceability.CloseRecall.CloseRecallEndpoint.Map(recalls);
    }
}
