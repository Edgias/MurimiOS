using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeEvidenceRequest;
public sealed record SupplierEvidenceCommandRequest(Guid? EvidenceAttachmentId, string? SupplierReference, string? Notes);
