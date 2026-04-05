namespace karaoke_place.Modules.Users.Models;

public class UserCreate
{
    public string Email { get; set; } = string.Empty;
    public string? Auth0Subject { get; set; }
}
