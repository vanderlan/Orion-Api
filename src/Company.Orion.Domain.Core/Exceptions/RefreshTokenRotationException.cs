using System;

namespace Company.Orion.Domain.Core.Exceptions;

/// <summary>
/// Thrown when the rotation of a refresh token cannot be completed, meaning the old token
/// could not be atomically revoked and replaced by a new one. When it happens the whole
/// rotation is rolled back, so the refresh token informed by the user remains valid and
/// the operation can be safely retried.
/// </summary>
[Serializable]
public class RefreshTokenRotationException : BusinessException
{
    public RefreshTokenRotationException(string message, string title) : base(message, title)
    {

    }

    public RefreshTokenRotationException(string message) : base(message)
    {

    }

    public RefreshTokenRotationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RefreshTokenRotationException(string message, string title, Exception innerException) : base(message, innerException)
    {
        Title = title;
    }
}
