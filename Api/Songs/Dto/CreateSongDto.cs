using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.Songs.Dto;

public class CreateSongDto
{
    [Required]
    [StringLength(200)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Artist { get; set; } = string.Empty;
}
