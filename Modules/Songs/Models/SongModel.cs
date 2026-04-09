namespace karaoke_place.Modules.Songs.Models;

public class SongModel
{
    public required int Id { get; set; }
    public required string ExternalId { get; set; } = string.Empty;
    public required string Title { get; set; } = string.Empty;
    public required string Artist { get; set; } = string.Empty;
}
