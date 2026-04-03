using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.KaraokeEvents.Dto;

public class EnterKaraokeEventDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}
