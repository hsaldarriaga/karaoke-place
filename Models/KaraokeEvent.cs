using System.ComponentModel.DataAnnotations.Schema;

namespace karaoke_place.Models;

[Table("KaraokeEvents")]
public class KaraokeEventDB
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public UserDB CreatedByUser { get; set; } = null!;
    public ICollection<EventParticipantDB> Participants { get; set; } = new List<EventParticipantDB>();
    public ICollection<SongProposalDB> SongProposals { get; set; } = new List<SongProposalDB>();
}
