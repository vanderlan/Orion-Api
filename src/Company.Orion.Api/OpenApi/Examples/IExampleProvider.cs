namespace Company.Orion.Api.OpenApi.Examples;

/// <summary>
/// Non-generic contract used by the OpenAPI pipeline to read an example without knowing its type.
/// Do not implement it directly: implement <see cref="IExampleProvider{T}"/> instead.
/// </summary>
public interface IExampleProvider
{
    object GetExampleValue();
}

/// <summary>
/// Provides a single example for the type <typeparamref name="T"/>.
/// Implementations placed in the API assembly are discovered automatically by
/// <see cref="ExamplesConfiguration.AddOpenApiExamples"/> and rendered in the API reference as the
/// request body / response example of every operation that uses <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The request, response or model type documented by the example.</typeparam>
public interface IExampleProvider<T> : IExampleProvider where T : class
{
    T GetExample();

    object IExampleProvider.GetExampleValue() => GetExample();
}
