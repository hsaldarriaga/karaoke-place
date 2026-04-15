namespace karaoke_place.Modules.KaraokeEvents;

internal class RawSongProposalRow
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public int SongId { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Song_Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
}
