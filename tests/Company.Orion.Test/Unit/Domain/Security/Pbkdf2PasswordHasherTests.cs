using System;
using Company.Orion.Domain.Core.Extensions;
using Company.Orion.Domain.Core.Security;
using Xunit;

namespace Company.Orion.Test.Unit.Domain.Security;

public class Pbkdf2PasswordHasherTests
{
    //A low work factor keeps the tests fast; the default (210k iterations) is covered by HashPassword_UsesTheDefaultWorkFactor.
    private readonly Pbkdf2PasswordHasher _hasher = new(iterations: 1_000);

    [Fact]
    public void Hash_ProducesASaltedSelfDescribingHash()
    {
        //arrange & act
        var first = _hasher.Hash("Str0ng#Passw0rd!");
        var second = _hasher.Hash("Str0ng#Passw0rd!");

        //assert
        var parts = first.Split('$');

        Assert.Equal(4, parts.Length);
        Assert.Equal(Pbkdf2PasswordHasher.AlgorithmName, parts[0]);
        Assert.Equal("1000", parts[1]);
        Assert.NotEqual(first, second); //random salt per password
        Assert.DoesNotContain("Str0ng#Passw0rd!", first);
    }

    [Fact]
    public void Hash_UsesTheDefaultWorkFactor()
    {
        //arrange & act
        var hash = new Pbkdf2PasswordHasher().Hash("Str0ng#Passw0rd!");

        //assert
        Assert.Equal(Pbkdf2PasswordHasher.DefaultIterations.ToString(), hash.Split('$')[1]);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Hash_WithEmptyPassword_Throws(string password)
    {
        Assert.ThrowsAny<ArgumentException>(() => _hasher.Hash(password));
    }

    [Fact]
    public void Verify_WithTheRightPassword_ReturnsSuccess()
    {
        //arrange
        var hash = _hasher.Hash("Str0ng#Passw0rd!");

        //act & assert
        Assert.Equal(PasswordVerificationResult.Success, _hasher.Verify(hash, "Str0ng#Passw0rd!"));
    }

    [Fact]
    public void Verify_WithTheWrongPassword_ReturnsFailed()
    {
        //arrange
        var hash = _hasher.Hash("Str0ng#Passw0rd!");

        //act & assert
        Assert.Equal(PasswordVerificationResult.Failed, _hasher.Verify(hash, "str0ng#passw0rd!"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("PBKDF2-SHA512$abc$salt$hash")]
    [InlineData("PBKDF2-SHA512$1000$not base64!$hash")]
    [InlineData("OTHER-ALGO$1000$c2FsdA==$aGFzaA==")]
    public void Verify_WithMalformedStoredHash_ReturnsFailedWithoutThrowing(string storedHash)
    {
        Assert.Equal(PasswordVerificationResult.Failed, _hasher.Verify(storedHash, "Str0ng#Passw0rd!"));
    }

    [Fact]
    public void Verify_WithAHashOfALowerWorkFactor_ReturnsSuccessRehashNeeded()
    {
        //arrange
        var weakerHash = new Pbkdf2PasswordHasher(iterations: 500).Hash("Str0ng#Passw0rd!");

        //act & assert
        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, _hasher.Verify(weakerHash, "Str0ng#Passw0rd!"));
    }

    [Fact]
    public void Verify_WithALegacySha512Hash_AcceptsThePasswordAndAsksForRehash()
    {
        //arrange: hash format used by the seeded admin user and by databases created before PBKDF2
        var legacyHash = "123".ToSha512();

        //act & assert
        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, _hasher.Verify(legacyHash, "123"));
        Assert.Equal(PasswordVerificationResult.Failed, _hasher.Verify(legacyHash, "1234"));
    }
}
