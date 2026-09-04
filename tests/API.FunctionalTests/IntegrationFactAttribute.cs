namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class IntegrationFactAttribute : FactAttribute
{
    public IntegrationFactAttribute()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("MURIMIOS_RUN_INTEGRATION_TESTS"), "1",
                StringComparison.Ordinal))
            Skip = "Set MURIMIOS_RUN_INTEGRATION_TESTS=1 and provide a migrated PostgreSQL test database.";
    }
}

public sealed class LoadFactAttribute : FactAttribute
{
    public LoadFactAttribute()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("MURIMIOS_RUN_LOAD_TESTS"), "1",
                StringComparison.Ordinal))
            Skip = "Set MURIMIOS_RUN_LOAD_TESTS=1 to run the bounded API load smoke test.";
    }
}
