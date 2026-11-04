namespace Company.Orion.Api.OpenApi.Examples.Shared;

/// <summary>
/// Deterministic values shared by the example providers, so the generated OpenAPI document is
/// stable between runs (handy for snapshot tests and client generators).
/// </summary>
internal static class ExampleData
{
    public const string AdminPublicId = "9b2f1c7e-4d3a-4c8e-9f1a-2b6d8e0c5a11";
    public const string OperatorPublicId = "3e7a9d21-0f4b-4b6c-8c2d-7a1e5f9b3c44";

    /// <summary>Credentials of the default admin user seeded on startup, so "Try it out" works out of the box.</summary>
    public const string AdminEmail = "adm@orion.com";
    public const string AdminPassword = "123";

    public const string OperatorEmail = "operator@orion.com";
    public const string Password = "Str0ng#Passw0rd!";

    /// <summary>A fake (but well formed) JWT, the real value is returned by POST /Auth/Login.</summary>
    public const string Jwt =
        "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9." +
        "eyJlbWFpbCI6ImFkbUBvcmlvbi5jb20iLCJnaXZlbl9uYW1lIjoiT3Jpb24gQWRtaW4gVXNlciIsInJvbGUiOiJhZG1pbiJ9." +
        "XbPfbIHMI6arZ3Y922BhjWgQzWXcXNrz0ogtVhfEd2o";

    /// <summary>A fake refresh token with the same shape of the real one (SHA-512 hex string).</summary>
    public const string RefreshToken =
        "c5f1d4a2e8b9f0a1b2c3d4e5f60718293a4b5c6d7e8f9012a3b4c5d6e7f8091a" +
        "2b3c4d5e6f708192a3b4c5d6e7f8091a2b3c4d5e6f708192a3b4c5d6e7f80910";

    public const string TraceId = "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01";

    public static readonly DateTime CreatedAt = new(2025, 1, 9, 14, 16, 7, DateTimeKind.Utc);
    public static readonly DateTime LastUpdated = new(2025, 3, 21, 9, 30, 0, DateTimeKind.Utc);
    public static readonly DateTime TokenExpiration = new(2025, 3, 21, 11, 30, 0, DateTimeKind.Utc);
}
