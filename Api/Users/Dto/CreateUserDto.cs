using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.Users.Dto;

public class CreateUserDto
{
    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
