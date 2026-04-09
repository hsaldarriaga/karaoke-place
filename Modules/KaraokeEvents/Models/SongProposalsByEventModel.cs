namespace karaoke_place.Modules.KaraokeEvents.Models;

public class SongProposalsByEventModel
{
    public required int EventId { get; set; }
    public required IEnumerable<SongProposalModel> SongProposals { get; set; } = [];
}
