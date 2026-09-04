using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder;
using AddResourceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.AddResource.AddResourceEndpoint;
using AssignEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Assign.AssignEndpoint;
using CalendarEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar.CalendarEndpoint;
using CancelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Cancel.CancelEndpoint;
using CompleteEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Complete.CompleteEndpoint;
using ConflictsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Conflicts.ConflictsEndpoint;
using CorrectEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Correct.CorrectEndpoint;
using MaterializePlanEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.MaterializePlan.MaterializePlanEndpoint;
using PlanEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Plan.PlanEndpoint;
using ReleaseResourceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.ReleaseResource.ReleaseResourceEndpoint;
using ReopenEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Reopen.ReopenEndpoint;
using StartEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Start.StartEndpoint;
using UnassignEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Unassign.UnassignEndpoint;
using UpdateDraftEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.UpdateDraft.UpdateDraftEndpoint;

using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using ListEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Audit.List.ListEndpoint;

public static class WorkOrderEndpoints
{
    public static void MapWorkOrderEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder operations = api.MapGroup("/operations").WithTags("Central Farm Operations");
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.List.ListEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create.CreateEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Get.GetEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.UpdateDraft.UpdateDraftEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Plan.PlanEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Assign.AssignEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Unassign.UnassignEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.AddResource.AddResourceEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.ReleaseResource.ReleaseResourceEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Start.StartEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Complete.CompleteEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Verify.VerifyEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Reject.RejectEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Reopen.ReopenEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Cancel.CancelEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Correct.CorrectEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.MaterializePlan.MaterializePlanEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar.CalendarEndpoint.Map(operations);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Conflicts.ConflictsEndpoint.Map(operations);
    }
}
