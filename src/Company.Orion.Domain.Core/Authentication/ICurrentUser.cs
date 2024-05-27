namespace Company.Orion.Domain.Core.Authentication;

public interface ICurrentUser
{
    string Name { get; }
    public string Id { get; }
    public string Email { get; }
    public string Profile { get; }
    bool IsAuthenticated();
}
