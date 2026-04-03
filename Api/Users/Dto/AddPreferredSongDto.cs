using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.Users.Dto;

public class AddPreferredSongDto
{
    [Range(1, int.MaxValue)]
    public int SongId { get; set; }
}
