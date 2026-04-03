using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.KaraokeEvents.Dto;

public class RespondInvitationDto
{
    [Range(1, int.MaxValue)]
    public int HostUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}
