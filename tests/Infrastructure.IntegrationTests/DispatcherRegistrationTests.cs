using EdgIQ.Dispatcher;
using Dispatcher.DependencyInjection;
using EdgIQ.MurimiOS.Application;
using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;

public sealed class DispatcherRegistrationTests
{
    [Fact]
    public void Application_registration_registers_dispatcher_services_and_production_handlers()
    {
        var services = new ServiceCollection();
        services.AddMurimiOSApplication();
        // Actual host construction and command/query resolution are checked by
        // ApiContractTests with the real persistence and request-context adapters.
        Assert.Contains(services, registration => registration.ServiceType == typeof(ISender));
        Assert.Contains(services, registration => registration.ServiceType == typeof(IPublisher));
        Assert.Contains(services, registration => registration.ServiceType == typeof(IDispatcher));
        Assert.Contains(services, registration => registration.ImplementationType ==
            typeof(EdgIQ.MurimiOS.Application.Audit.List.ListHandler));
    }

    [Fact]
    public async Task Assembly_scanning_resolves_command_and_query_handlers()
    {
        var services = new ServiceCollection();
        services.AddMurimiOSApplication();
        // Fixtures stay in the test assembly; production scans Application only.
        services.AddDispatcher(options =>
            options.RegisterServicesFromAssemblyContaining<DispatcherRegistrationTests>());
        services.AddSingleton<CommandState>();
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        using var cancellation = new CancellationTokenSource();

        await sender.Send(new SmokeCommand("command handled"), cancellation.Token);
        string result = await sender.Send(new SmokeQuery(), cancellation.Token);

        Assert.Equal("command handled", result);
        Assert.Equal(cancellation.Token, provider.GetRequiredService<CommandState>().CancellationToken);
    }

    public sealed record SmokeCommand(string Value) : IRequest;
    public sealed record SmokeQuery : IRequest<string>;

    public sealed class CommandState
    {
        public string Value { get; set; } = string.Empty;
        public CancellationToken CancellationToken { get; set; }
    }

    public sealed class SmokeCommandHandler(CommandState state) : IRequestHandler<SmokeCommand>
    {
        public Task Handle(SmokeCommand request, CancellationToken cancellationToken)
        {
            state.Value = request.Value;
            state.CancellationToken = cancellationToken;
            return Task.CompletedTask;
        }
    }

    public sealed class SmokeQueryHandler(CommandState state) : IRequestHandler<SmokeQuery, string>
    {
        public Task<string> Handle(SmokeQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(state.Value);
    }
}
