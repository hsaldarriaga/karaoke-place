using System.ComponentModel.DataAnnotations.Schema;

namespace karaoke_place.Models;

[Table("Users")]
public class UserDB
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<KaraokeEventDB> CreatedEvents { get; set; } = new List<KaraokeEventDB>();
    public ICollection<EventParticipantDB> EventParticipations { get; set; } = new List<EventParticipantDB>();
    public ICollection<SongProposalDB> SongProposals { get; set; } = new List<SongProposalDB>();
    public ICollection<UserPreferredSongDB> PreferredSongs { get; set; } = new List<UserPreferredSongDB>();
}
