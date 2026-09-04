using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using TenantDatabaseRoutingScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.TenantDatabaseRoutingScope;
using TenantDatabaseOptions = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseOptions;
using TenantDatabaseRegistry = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseRegistry;
using TenantMigrationExecutionOptions = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantMigrationExecutionOptions;

public sealed class TenantDatabaseRoutingTests
{
    private const string Control = "Host=control;Database=murimios;Username=app;Password=secret";
    private const string Dedicated = "Host=dedicated;Database=tenant_one;Username=app;Password=secret";

    [Fact]
    public void Registry_resolves_named_connections_and_rejects_unknown_database_keys()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DedicatedOne"] = Dedicated
        }).Build();
        var options = Options.Create(new TenantDatabaseOptions
        {
            ControlDatabaseKey = "primary",
            Connections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["tenant-one"] = "DedicatedOne"
            }
        });

        var registry = new TenantDatabaseRegistry(options, configuration, Control);

        Assert.True(registry.Contains("PRIMARY"));
        Assert.True(registry.Contains("tenant-one"));
        Assert.Equal("tenant_one", new Npgsql.NpgsqlConnectionStringBuilder(registry.GetConnectionString("tenant-one")).Database);
        Assert.Throws<InvalidOperationException>(() => registry.GetConnectionString("missing"));
    }

    [Fact]
    public void Control_plane_scope_is_nested_and_disposes_idempotently()
    {
        var scope = new TenantDatabaseRoutingScope();
        Assert.False(scope.UseControlDatabase);
        IDisposable first = scope.EnterControlPlane();
        IDisposable second = scope.EnterControlPlane();
        Assert.True(scope.UseControlDatabase);
        second.Dispose();
        Assert.True(scope.UseControlDatabase);
        second.Dispose();
        first.Dispose();
        Assert.False(scope.UseControlDatabase);
    }

    [Fact]
    public void Execution_options_fail_closed_for_unsafe_limits()
    {
        new TenantMigrationExecutionOptions().Validate();
        Assert.Throws<InvalidOperationException>(() => new TenantMigrationExecutionOptions { LeaseSeconds = 5 }.Validate());
        Assert.Throws<InvalidOperationException>(() => new TenantMigrationExecutionOptions { MaximumRecordBytes = 1024 }.Validate());
        Assert.Throws<InvalidOperationException>(() => new TenantMigrationExecutionOptions { WriteDrainSeconds = 61 }.Validate());
    }
}
