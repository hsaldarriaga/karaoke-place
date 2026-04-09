using System;

namespace karaoke_place.Modules.Users.Models;

public class UserModel
{
    public required int Id { get; set; }
    public required string Email { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
}
