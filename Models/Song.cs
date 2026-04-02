namespace karaoke_place.Models;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<SongProposal> SongProposals { get; set; } = new List<SongProposal>();
}
