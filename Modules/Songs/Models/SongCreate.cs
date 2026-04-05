namespace karaoke_place.Modules.Songs.Models;

public class SongCreate
{
    public string ExternalId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
}
