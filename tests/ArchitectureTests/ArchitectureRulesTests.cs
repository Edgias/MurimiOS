using System.Reflection;
using System.Text.RegularExpressions;
using EdgIQ.MurimiOS.Application;
using EdgIQ.MurimiOS.Domain.Common;
using NetArchTest.Rules;

namespace EdgIQ.MurimiOS.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationAssemblyMarker).Assembly;

    [Fact]
    public void Domain_has_no_application_infrastructure_api_or_persistence_dependency() => AssertRule(
        Types.InAssembly(typeof(BaseEntity).Assembly).ShouldNot().HaveDependencyOnAny(
            "EdgIQ.MurimiOS.Application", "EdgIQ.MurimiOS.Infrastructure", "EdgIQ.MurimiOS.API",
            "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore").GetResult());

    [Fact]
    public void Application_has_no_infrastructure_api_or_http_dependency() => AssertRule(
        Types.InAssembly(ApplicationAssembly).ShouldNot().HaveDependencyOnAny(
            "EdgIQ.MurimiOS.Infrastructure", "EdgIQ.MurimiOS.API", "Microsoft.AspNetCore").GetResult());

    [Fact]
    public void Infrastructure_does_not_depend_on_the_http_host() => AssertRule(
        Types.InAssembly(typeof(EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext).Assembly)
            .ShouldNot().HaveDependencyOn("EdgIQ.MurimiOS.API").GetResult());

    [Fact]
    public void Only_the_api_composition_root_may_depend_on_infrastructure() => AssertRule(
        Types.InAssembly(typeof(Program).Assembly).That().ResideInNamespaceMatching(@"^EdgIQ\.MurimiOS\.API(?:\.|$)")
            .ShouldNot().HaveDependencyOn("EdgIQ.MurimiOS.Infrastructure").GetResult());

    [Fact]
    public void Application_use_cases_have_no_cross_slice_dependencies()
    {
        const string prefix = "EdgIQ.MurimiOS.Application.";
        string[] slices = ApplicationAssembly.GetTypes().Select(t => t.Namespace)
            .Where(ns => ns is not null && ns.StartsWith(prefix, StringComparison.Ordinal))
            .Select(ns => ns![prefix.Length..].Split('.'))
            .Where(parts => parts.Length >= 2 && parts[0] != "Common" && parts[1] != "Shared")
            .Select(parts => prefix + parts[0] + "." + parts[1]).Distinct().Order().ToArray();
        Assert.Equal(590, slices.Length);
        foreach (string slice in slices)
        {
            string[] otherSlices = slices.Where(other => other != slice).Select(other => other + ".").ToArray();
            AssertRule(Types.InAssembly(ApplicationAssembly).That().ResideInNamespaceMatching("^" + Regex.Escape(slice) + @"(?:\.|$)")
                .ShouldNot().HaveDependencyOnAny(otherSlices).GetResult(), slice);
        }
    }

    private static void AssertRule(TestResult result, string? scope = null) => Assert.True(result.IsSuccessful,
        $"Architecture rule failed for {scope ?? "layer"}: {string.Join(", ", result.FailingTypeNames ?? [])}");
}
