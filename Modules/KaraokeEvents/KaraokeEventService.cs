using karaoke_place.Modules.Common;
using karaoke_place.Modules.KaraokeEvents.Models;

namespace karaoke_place.Modules.KaraokeEvents;

public class KaraokeEventService(KaraokeEventRepository repo)
{
    private readonly KaraokeEventRepository _repo = repo;

    public Task<PagedResult<KaraokeEvent>> GetAllAsync(bool? isActive = null, int page = 1, int pageSize = 20)
    {
        return _repo.GetAllAsync(isActive, page, pageSize);
    }

    public Task<KaraokeEvent?> GetByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }

    public Task<IEnumerable<EventParticipantModel>> GetParticipantsAsync(int eventId)
    {
        return _repo.GetParticipantsAsync(eventId);
    }

    public Task<IEnumerable<SongProposalModel>> GetSongProposalsAsync(int eventId)
    {
        return _repo.GetSongProposalsAsync(eventId);
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
