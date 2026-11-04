namespace Company.Orion.Api.OpenApi.Examples;

/// <summary>
/// Type-agnostic view of a named example, used internally by the OpenAPI pipeline.
/// </summary>
public interface INamedExample
{
    string Name { get; }
    string Summary { get; }
    string Description { get; }
    object Value { get; }
}

/// <summary>
/// A named example returned by <see cref="IMultipleExamplesProvider{T}"/>.
/// </summary>
/// <param name="Name">The key shown in the API reference examples dropdown. Must be unique per provider.</param>
/// <param name="Value">The example payload.</param>
/// <param name="Summary">Optional short description shown next to the name.</param>
/// <param name="Description">Optional long description.</param>
public sealed record NamedExample<T>(string Name, T Value, string Summary = null, string Description = null) : INamedExample
    where T : class
{
    object INamedExample.Value => Value;
}
