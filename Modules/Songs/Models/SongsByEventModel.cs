using karaoke_place.Modules.Common;

namespace karaoke_place.Modules.Songs.Models;

public class SongsByEventModel
{
    public required int EventId { get; set; }
    public required PagedResult<SongByEventModel> Songs { get; set; } = null!;
}
