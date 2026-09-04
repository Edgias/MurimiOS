namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface IOrganisationScopeAccess
{
    Task<ResolvedOrganisationScope> ResolveAsync(CancellationToken cancellationToken);
    Task<bool> CanAccessFarmAsync(Guid farmId, CancellationToken cancellationToken);
}
