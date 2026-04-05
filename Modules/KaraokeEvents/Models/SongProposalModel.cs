namespace karaoke_place.Modules.KaraokeEvents.Models;

public class SongProposalModel
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public int SongId { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
}
