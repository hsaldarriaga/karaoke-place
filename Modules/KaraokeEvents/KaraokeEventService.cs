using karaoke_place.Modules.KaraokeEvents.Models;

namespace karaoke_place.Modules.KaraokeEvents;

public class KaraokeEventService(KaraokeEventRepository repo)
{
    private readonly KaraokeEventRepository _repo = repo;

    public Task<IEnumerable<KaraokeEvent>> GetAllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<KaraokeEvent?> GetByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }

    public Task<KaraokeEvent> CreateAsync(KaraokeEventCreate model)
    {
        return _repo.AddAsync(model);
    }

    public Task<bool> UpdateAsync(int id, KaraokeEventUpdate model)
    {
        return _repo.UpdateAsync(id, model);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _repo.DeleteAsync(id);
    }

    public async Task<(bool ok, string? error)> PublishAsync(int id)
    {
        var now = DateTime.UtcNow;
        var (model, error) = await _repo.UpdateIsActive(id, now, true);
        if (error == "NotFound") return (false, "NotFound");
        if (error == "EndTimePassed") return (false, "EndTimePassed");
        if (model == null) return (false, "Error");
        return (true, null);
    }

    public Task<(bool ok, string? error)> EnterKaraokeEventAsync(int eventId, int userId)
    {
        return _repo.EnterKaraokeEventAsync(eventId, userId);
    }

    public Task<(bool ok, string? error)> AcceptInvitationAsync(int eventId, int hostUserId, int userId)
    {
        return _repo.RespondInvitationAsync(eventId, hostUserId, userId, ParticipantStatus.Accepted);
    }

    public Task<(bool ok, string? error)> RejectInvitationAsync(int eventId, int hostUserId, int userId)
    {
        return _repo.RespondInvitationAsync(eventId, hostUserId, userId, ParticipantStatus.Rejected);
    }
}
