using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow;
using AcceptInvitationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.AcceptInvitation.AcceptInvitationEndpoint;
using ChangeAllocationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeAllocation.ChangeAllocationEndpoint;
using ChangeContractEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeContract.ChangeContractEndpoint;
using ChangeEvidenceRequestEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeEvidenceRequest.ChangeEvidenceRequestEndpoint;
using ChangeSupplierEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeSupplier.ChangeSupplierEndpoint;
using CreateAllocationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateAllocation.CreateAllocationEndpoint;
using CreateContractEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateContract.CreateContractEndpoint;
using CreateEvidenceRequestEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateEvidenceRequest.CreateEvidenceRequestEndpoint;
using CreateInvitationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateInvitation.CreateInvitationEndpoint;
using CreateSupplierEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateSupplier.CreateSupplierEndpoint;
using ListContractsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListContracts.ListContractsEndpoint;
using ListEvidenceRequestsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListEvidenceRequests.ListEvidenceRequestsEndpoint;
using ListInvitationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListInvitations.ListInvitationsEndpoint;
using ListSuppliersEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListSuppliers.ListSuppliersEndpoint;
using RevokeInvitationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.RevokeInvitation.RevokeInvitationEndpoint;
using SupplierStatusEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierStatus.SupplierStatusEndpoint;


public static class SupplierWorkflowEndpoints
{
    public static void MapSupplierWorkflowEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder portal = api.MapGroup("/supplier-portal").WithTags("Supplier portal");
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListInvitations.ListInvitationsEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateInvitation.CreateInvitationEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.RevokeInvitation.RevokeInvitationEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.AcceptInvitation.AcceptInvitationEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListSuppliers.ListSuppliersEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateSupplier.CreateSupplierEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeSupplier.ChangeSupplierEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierStatus.SupplierStatusEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListContracts.ListContractsEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateContract.CreateContractEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeContract.ChangeContractEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateAllocation.CreateAllocationEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeAllocation.ChangeAllocationEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListEvidenceRequests.ListEvidenceRequestsEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateEvidenceRequest.CreateEvidenceRequestEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeEvidenceRequest.ChangeEvidenceRequestEndpoint.Map(portal);
    }
}
