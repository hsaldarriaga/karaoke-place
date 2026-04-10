using karaoke_place.Modules.Songs.Models;

namespace karaoke_place.Modules.KaraokeEvents.Models;

public class SongProposalModel
{
    public required int Id { get; set; }
    public required int EventId { get; set; }
    public required int UserId { get; set; }
    public required int SongId { get; set; }
    public required int Order { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required SongModel Song { get; set; } = null!;
}
