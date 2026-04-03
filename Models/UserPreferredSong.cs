using System.ComponentModel.DataAnnotations.Schema;

namespace karaoke_place.Models;

[Table("UserPreferredSongs")]
public class UserPreferredSongDB
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SongId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserDB User { get; set; } = null!;
    public SongDB Song { get; set; } = null!;
}
