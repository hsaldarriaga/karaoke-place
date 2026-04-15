using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.Users.Dto;

public class AddPreferredSongDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Artist { get; set; } = string.Empty;
}
