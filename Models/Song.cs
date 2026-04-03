using System.ComponentModel.DataAnnotations.Schema;

namespace karaoke_place.Models;

[Table("Songs")]
public class SongDB
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<SongProposalDB> SongProposals { get; set; } = new List<SongProposalDB>();
    public ICollection<UserPreferredSongDB> PreferredByUsers { get; set; } = new List<UserPreferredSongDB>();
}
