using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateEvidenceRequest;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;

using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;
public sealed record CreateSupplierEvidenceRequestRequest(Guid? Id, Guid? RelationshipId, Guid? HostedSupplierId,
    Guid? ContractId, Guid? StandardVersionId, Guid? ControlPointId, string Title, string Description, DateOnly DueOn);
