namespace Company.Orion.Api.OpenApi.Examples;

/// <summary>
/// Non-generic contract used by the OpenAPI pipeline to read a response example without knowing its type.
/// Do not implement it directly: implement <see cref="IResponseExampleProvider{T}"/> instead.
/// </summary>
public interface IResponseExampleProvider
{
    object GetExampleValue(int statusCode);
}

/// <summary>
/// Provides a response example for the type <typeparamref name="T"/> that depends on the HTTP status
/// code of the response (e.g. <c>ProblemDetails</c> is documented for 400, 401, 404 and 409 with a
/// different payload each). For status-independent examples prefer <see cref="IExampleProvider{T}"/>.
/// </summary>
/// <typeparam name="T">The response type documented by the example.</typeparam>
public interface IResponseExampleProvider<T> : IResponseExampleProvider where T : class
{
    T GetExample(int statusCode);

    object IResponseExampleProvider.GetExampleValue(int statusCode) => GetExample(statusCode);
}
