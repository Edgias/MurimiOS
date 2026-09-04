using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Access.Update;

public sealed record UpdateCommand(Guid membershipId, UpdateMembershipAccessRequest request) : IRequest<UseCaseResult>;
