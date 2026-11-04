namespace Company.Orion.Domain.Core.Security;

public enum PasswordVerificationResult
{
    /// <summary>The password does not match the stored hash.</summary>
    Failed = 0,

    /// <summary>The password matches the stored hash.</summary>
    Success = 1,

    /// <summary>
    /// The password matches, but the stored hash uses an outdated algorithm or work factor and
    /// should be replaced by <see cref="IPasswordHasher.Hash"/> of the same password.
    /// </summary>
    SuccessRehashNeeded = 2
}

/// <summary>
/// Hashes and verifies user passwords. Passwords are never stored or compared in plain text and
/// never looked up by hash: load the user by e-mail and call <see cref="Verify"/>.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    PasswordVerificationResult Verify(string? hashedPassword, string? providedPassword);
}
