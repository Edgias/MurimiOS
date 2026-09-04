using EdgIQ.MurimiOS.Application;

namespace EdgIQ.MurimiOS.Application.UnitTests;

public sealed class ApplicationDependencyTests
{
    [Fact]
    public void Application_has_no_http_or_infrastructure_assembly_dependency()
    {
        string[] references = typeof(ApplicationAssemblyMarker).Assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty).ToArray();
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
        Assert.DoesNotContain("Infrastructure", references);
        Assert.DoesNotContain("API", references);
    }
}
