using Orion.Domain.Entities.Enuns;

namespace Orion.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserProfile Profile { get; set; }
}
