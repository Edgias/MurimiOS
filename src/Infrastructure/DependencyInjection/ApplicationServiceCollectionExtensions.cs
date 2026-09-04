using Dispatcher.DependencyInjection;
using EdgIQ.MurimiOS.Application;
using Microsoft.Extensions.DependencyInjection;

namespace EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using ApplicationAssemblyMarker = global::EdgIQ.MurimiOS.Application.ApplicationAssemblyMarker;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddMurimiOSApplication(this IServiceCollection services)
    {
        services.AddDispatcher(options =>
            options.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>());
        return services.AddApplicationPersistenceContracts();
    }

    public static IServiceCollection AddApplicationPersistenceContracts(this IServiceCollection services)
    {
        services.AddScoped<EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext>(provider =>
            provider.GetRequiredService<EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext>());
        services.AddScoped<EdgIQ.MurimiOS.Application.Common.Abstractions.ILegacyDbContext>(provider =>
            provider.GetRequiredService<EdgIQ.MurimiOS.Infrastructure.Data.MurimiOSDbContext>());
        return services;
    }
}
