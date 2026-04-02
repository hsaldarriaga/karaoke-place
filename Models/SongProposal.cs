namespace karaoke_place.Models;

public enum SongProposalStatus
{
    Proposed,
    Approved,
    Rejected,
    Performed
}

public class SongProposal
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public int? SongId { get; set; }
    public string SongTitle { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public SongProposalStatus Status { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public KaraokeEvent Event { get; set; } = null!;
    public User User { get; set; } = null!;
    public Song? Song { get; set; }
}
