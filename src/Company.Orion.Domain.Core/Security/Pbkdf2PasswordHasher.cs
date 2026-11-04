using System.Security.Cryptography;
using System.Text;
using Company.Orion.Domain.Core.Extensions;

namespace Company.Orion.Domain.Core.Security;

/// <summary>
/// PBKDF2-HMAC-SHA512 password hasher (<see cref="Rfc2898DeriveBytes.Pbkdf2(string, byte[], int, HashAlgorithmName, int)"/>)
/// with a random 128-bit salt per password and a work factor aligned with the OWASP recommendation.
/// Stored format: <c>PBKDF2-SHA512$iterations$salt(base64)$hash(base64)</c>, so the work factor can be
/// raised later: older hashes keep validating and are reported as <see cref="PasswordVerificationResult.SuccessRehashNeeded"/>.
/// Legacy unsalted SHA-512 hashes (128 hex chars, used by the seeded data and by databases created
/// before this hasher) are still accepted and reported for rehash, so no user is locked out.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    public const string AlgorithmName = "PBKDF2-SHA512";
    public const int DefaultIterations = 210_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int LegacySha512HexLength = 128;
    private const char Separator = '$';

    private readonly int _iterations;

    public Pbkdf2PasswordHasher() : this(DefaultIterations)
    {
    }

    public Pbkdf2PasswordHasher(int iterations)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(iterations, 1);
        _iterations = iterations;
    }

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Derive(password, salt, _iterations);

        return string.Join(Separator, AlgorithmName, _iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public PasswordVerificationResult Verify(string? hashedPassword, string? providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            return PasswordVerificationResult.Failed;

        if (IsLegacySha512(hashedPassword))
            return VerifyLegacySha512(hashedPassword, providedPassword);

        var parts = hashedPassword.Split(Separator);

        if (parts.Length != 4 || parts[0] != AlgorithmName || !int.TryParse(parts[1], out var iterations) || iterations < 1)
            return PasswordVerificationResult.Failed;

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            expectedHash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return PasswordVerificationResult.Failed;
        }

        var actualHash = Derive(providedPassword, salt, iterations, expectedHash.Length);

        if (!CryptographicOperations.FixedTimeEquals(actualHash, expectedHash))
            return PasswordVerificationResult.Failed;

        return iterations < _iterations
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Success;
    }

    private static byte[] Derive(string password, byte[] salt, int iterations, int length = HashSize)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, length);
    }

    private static bool IsLegacySha512(string hashedPassword)
    {
        if (hashedPassword.Length != LegacySha512HexLength)
            return false;

        foreach (var character in hashedPassword)
        {
            if (!Uri.IsHexDigit(character))
                return false;
        }

        return true;
    }

    private static PasswordVerificationResult VerifyLegacySha512(string hashedPassword, string providedPassword)
    {
        var expected = Encoding.UTF8.GetBytes(hashedPassword);
        var actual = Encoding.UTF8.GetBytes(providedPassword.ToSha512());

        return CryptographicOperations.FixedTimeEquals(actual, expected)
            ? PasswordVerificationResult.SuccessRehashNeeded
            : PasswordVerificationResult.Failed;
    }
}
