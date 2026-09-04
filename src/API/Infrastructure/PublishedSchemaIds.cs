using EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdgIQ.MurimiOS.API.Infrastructure;
using GetAccessMeResponse = global::EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe.GetAccessMeResponse;

public static class PublishedSchemaIds
{
    private static readonly Func<Type, string> DefaultSelector = new SchemaGeneratorOptions().SchemaIdSelector;

    public static string Select(Type type)
    {
        if (type == typeof(GetAccessMeResponse))
            return "GuidStringGuidNullableStringGuidNullableStringIOrderedEnumerable<>f__AnonymousType0";
        // Compiler-generated anonymous ordinals change when unrelated bodies move.
        // Keep the already-published protocol-info schema ID stable across assemblies.
        if ((type.Name.Contains("AnonymousType", StringComparison.Ordinal) || type.Name == "PlatformInfoResponse")
            && type.GetProperties().Select(p => p.Name).Order(StringComparer.Ordinal)
                .SequenceEqual(new[] { "apiVersion", "name", "syncCommands", "syncSchemaVersions" }))
            return "StringStringInt32ArrayStringArray<>f__AnonymousType248";
        return DefaultSelector(type);
    }
}
