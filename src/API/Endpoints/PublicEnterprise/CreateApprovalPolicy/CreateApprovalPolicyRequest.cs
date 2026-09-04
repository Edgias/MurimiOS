using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalPolicy;
public sealed record CreateApprovalPolicyRequest(Guid? Id, string Code, string Name, Guid OrganisationNodeId, string ActionType, string ApproverPermission, int RequiredApprovals);
