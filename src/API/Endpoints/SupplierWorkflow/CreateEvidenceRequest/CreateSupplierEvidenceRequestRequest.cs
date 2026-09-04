using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateEvidenceRequest;
public sealed record CreateSupplierEvidenceRequestRequest(Guid? Id, Guid? RelationshipId, Guid? HostedSupplierId,
    Guid? ContractId, Guid? StandardVersionId, Guid? ControlPointId, string Title, string Description, DateOnly DueOn);
