namespace Company.Orion.Api.OpenApi.Examples;

/// <summary>
/// Non-generic contract used by the OpenAPI pipeline to read the examples without knowing their type.
/// Do not implement it directly: implement <see cref="IMultipleExamplesProvider{T}"/> instead.
/// </summary>
public interface IMultipleExamplesProvider
{
    IEnumerable<INamedExample> GetNamedExamples();
}

/// <summary>
/// Provides several named examples for the type <typeparamref name="T"/>. API reference UIs (Scalar) render them
/// as a dropdown, which is handy to document different scenarios of the same endpoint
/// (e.g. "Admin user" and "Operator user").
/// </summary>
/// <typeparam name="T">The request, response or model type documented by the examples.</typeparam>
public interface IMultipleExamplesProvider<T> : IMultipleExamplesProvider where T : class
{
    IEnumerable<NamedExample<T>> GetExamples();

    IEnumerable<INamedExample> IMultipleExamplesProvider.GetNamedExamples() => GetExamples();
}
