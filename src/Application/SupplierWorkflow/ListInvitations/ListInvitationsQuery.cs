using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ListInvitations;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
using SupplierInvitationStatus = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitationStatus;

public sealed record ListInvitationsQuery(SupplierInvitationStatus? status) : IRequest<UseCaseResult>;
