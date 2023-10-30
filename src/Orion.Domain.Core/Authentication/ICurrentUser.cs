namespace Orion.Domain.Core.Authentication;

public interface ICurrentUser
{
    string Name { get; }
    bool IsAuthenticated();
}
