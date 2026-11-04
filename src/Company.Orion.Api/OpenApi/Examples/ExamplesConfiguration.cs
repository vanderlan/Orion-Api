using System.Reflection;

namespace Company.Orion.Api.OpenApi.Examples;

public static class ExamplesConfiguration
{
    /// <summary>
    /// Scans the given assemblies (the API assembly when none is informed) and registers every
    /// <see cref="IExampleProvider{T}"/>, <see cref="IMultipleExamplesProvider{T}"/> and
    /// <see cref="IResponseExampleProvider{T}"/> found, plus the <see cref="OpenApiExamplesApplier"/>
    /// used by the OpenAPI operation transformer. Providers are resolved from the container,
    /// therefore they can receive dependencies (e.g. <c>IStringLocalizer</c>) through constructor injection.
    /// </summary>
    public static IServiceCollection AddOpenApiExamples(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = [typeof(ExamplesConfiguration).Assembly];

        var providers = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .SelectMany(type => type.GetInterfaces()
                .Where(IsExampleProviderContract)
                .Select(contract => (Contract: contract, Implementation: type)));

        foreach (var (contract, implementation) in providers)
        {
            services.AddTransient(contract, implementation);
        }

        //The applier serializes with the MVC JsonOptions; AddOptions guarantees IOptions<T> is resolvable
        //even when the examples are registered on a bare container (e.g. unit tests).
        services.AddOptions();
        services.AddSingleton<OpenApiExamplesApplier>();

        return services;
    }

    private static bool IsExampleProviderContract(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var definition = type.GetGenericTypeDefinition();

        return definition == typeof(IExampleProvider<>)
            || definition == typeof(IMultipleExamplesProvider<>)
            || definition == typeof(IResponseExampleProvider<>);
    }
}
