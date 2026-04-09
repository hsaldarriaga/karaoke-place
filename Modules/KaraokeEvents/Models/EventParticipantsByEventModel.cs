namespace karaoke_place.Modules.KaraokeEvents.Models;

public class EventParticipantsByEventModel
{
    public required int EventId { get; set; }
    public required IEnumerable<EventParticipantModel> Participants { get; set; } = [];
}
