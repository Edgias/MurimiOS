using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.DecideApproval;
using ApprovalDecisionKind = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecisionKind;
public sealed record ApprovalDecisionRequest(ApprovalDecisionKind Decision, string? Reason);
