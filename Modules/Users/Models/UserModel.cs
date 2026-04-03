using System;

namespace karaoke_place.Modules.Users.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
