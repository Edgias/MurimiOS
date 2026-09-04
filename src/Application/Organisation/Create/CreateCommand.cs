using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Organisation.Create;

public sealed record CreateCommand(CreateOrganisationNodeRequest request) : IRequest<UseCaseResult>;
