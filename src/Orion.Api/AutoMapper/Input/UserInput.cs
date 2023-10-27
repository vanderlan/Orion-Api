using Orion.Core.Domain.Entities.Enuns;

namespace Orion.Api.AutoMapper.Input;

public class UserInput
{
    public string PublicId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserProfile Profile { get; set; }
}
