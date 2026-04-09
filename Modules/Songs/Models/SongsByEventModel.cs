namespace karaoke_place.Modules.Songs.Models;

public class SongsByEventModel
{
    public required int EventId { get; set; }
    public required IEnumerable<SongModel> Songs { get; set; } = [];
}
