﻿using Microsoft.AspNetCore.Http.Metadata;
using MiniValidation;
using System.Reflection;

namespace Edgias.MurimiOS.API.Filters;

public static class ValidationFilterExtensions
{
    public static TBuilder WithParameterValidation<TBuilder>(this TBuilder builder, params Type[] typesToValidate) where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(eb =>
        {
            var methodInfo = eb.Metadata.OfType<MethodInfo>().FirstOrDefault();

            if (methodInfo is null)
            {
                return;
            }

            // Track the indices of validatable parameters
            List<int>? parameterIndexesToValidate = null;
            foreach (var p in methodInfo.GetParameters())
            {
                if (typesToValidate.Contains(p.ParameterType))
                {
                    parameterIndexesToValidate ??= new();
                    parameterIndexesToValidate.Add(p.Position);
                }
            }

            if (parameterIndexesToValidate is null)
            {
                // Nothing to validate so don't add the filter to this endpoint
                return;
            }

            // We can respond with problem details if there's a validation error
            eb.Metadata.Add(new ProducesResponseTypeMetadata(typeof(HttpValidationProblemDetails), 400, "application/problem+json"));

            eb.FilterFactories.Add((context, next) =>
            {
                return efic =>
                {
                    foreach (var index in parameterIndexesToValidate)
                    {
                        if (efic.Arguments[index] is { } arg && !MiniValidator.TryValidate(arg, out var errors))
                        {
                            return new ValueTask<object?>(Results.ValidationProblem(errors));
                        }
                    }

                    return next(efic);
                };
            });
        });

        return builder;
    }

    // Equivalent to the .Produces call to add metadata to endpoints
    private sealed class ProducesResponseTypeMetadata(Type type, int statusCode, string contentType) : IProducesResponseTypeMetadata
    {
        public Type Type { get; } = type;
        public int StatusCode { get; } = statusCode;
        public IEnumerable<string> ContentTypes { get; } = new[] { contentType };
    }
}

