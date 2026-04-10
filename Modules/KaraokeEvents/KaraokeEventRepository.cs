using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Common;
using karaoke_place.Modules.KaraokeEvents.Models;
using karaoke_place.Modules.Songs.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.KaraokeEvents;

public class KaraokeEventRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<PagedResult<KaraokeEvent>> GetAllAsync(bool? isActive = null, int page = 1, int pageSize = 20)
    {
        var query = _db.KaraokeEvents.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(e => e.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new KaraokeEvent
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Location = e.Location,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CreatedByUserId = e.CreatedByUserId,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<KaraokeEvent>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<KaraokeEvent?> GetByIdAsync(int id)
    {
        var e = await _db.KaraokeEvents.FindAsync(id);
        if (e == null) return null;
        return new KaraokeEvent
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            CreatedByUserId = e.CreatedByUserId,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        };
    }

    public async Task<IEnumerable<EventParticipantsByEventModel>> GetParticipantsAsync(IEnumerable<int> eventIds)
    {
        var eventIdList = eventIds.Distinct().ToList();

        var participants = await _db.EventParticipants
            .AsNoTracking()
            .Where(ep => eventIdList.Contains(ep.EventId))
            .OrderBy(ep => ep.CreatedAt)
            .Select(ep => new EventParticipantModel
            {
                Id = ep.Id,
                EventId = ep.EventId,
                UserId = ep.UserId,
                Role = ep.Role,
                Status = ep.Status,
                CreatedAt = ep.CreatedAt
            })
            .ToListAsync();

        var participantsByEventId = participants
            .GroupBy(ep => ep.EventId)
            .ToDictionary(group => group.Key, group => (IEnumerable<EventParticipantModel>)group.ToList());

        return eventIdList.Select(eventId => new EventParticipantsByEventModel
        {
            EventId = eventId,
            Participants = participantsByEventId.GetValueOrDefault(eventId, [])
        });
    }

    public async Task<IEnumerable<SongProposalsByEventModel>> GetSongProposalsAsync(IEnumerable<int> eventIds, int limitPerEvent = 20)
    {
        var eventIdList = eventIds.Distinct().ToList();
        if (eventIdList.Count == 0) return [];

        var proposals = await _db.SongProposals
            .AsNoTracking()
            .Where(sp => eventIdList.Contains(sp.EventId))
            .GroupBy(sp => sp.EventId)
            .SelectMany(group => group
                .OrderBy(sp => sp.Order)
                .ThenBy(sp => sp.CreatedAt)
                .ThenBy(sp => sp.Id)
                .Take(limitPerEvent))
            .Select(sp => new SongProposalModel
            {
                Id = sp.Id,
                EventId = sp.EventId,
                UserId = sp.UserId,
                SongId = sp.SongId,
                Order = sp.Order,
                CreatedAt = sp.CreatedAt,
                Song = new SongModel
                {
                    Id = sp.Song.Id,
                    ExternalId = sp.Song.ExternalId,
                    Title = sp.Song.Title,
                    Artist = sp.Song.Artist
                }
            })
            .ToListAsync();

        var proposalsByEventId = proposals
            .GroupBy(sp => sp.EventId)
            .ToDictionary(group => group.Key, group => (IEnumerable<SongProposalModel>)group.ToList());

        return eventIdList.Select(eventId => new SongProposalsByEventModel
        {
            EventId = eventId,
            SongProposals = proposalsByEventId.GetValueOrDefault(eventId, [])
        });
    }

    public async Task<KaraokeEvent> AddAsync(KaraokeEventCreate model)
    {
        var e = new KaraokeEventDB
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Location = model.Location,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            CreatedByUserId = model.CreatedByUserId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.KaraokeEvents.Add(e);
        await _db.SaveChangesAsync();

        return new KaraokeEvent
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            CreatedByUserId = e.CreatedByUserId,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, KaraokeEventUpdate model)
    {
        var e = await _db.KaraokeEvents.FindAsync(id);
        if (e == null) return false;

        e.Name = model.Name;
        e.Description = model.Description ?? string.Empty;
        e.Location = model.Location;
        e.StartTime = model.StartTime;
        e.EndTime = model.EndTime;
        e.CreatedByUserId = model.CreatedByUserId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _db.KaraokeEvents.FindAsync(id);
        if (e == null) return false;
        _db.KaraokeEvents.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(KaraokeEvent? model, string? error)> UpdateIsActive(int id, DateTime now, bool isActive)
    {
        var e = await _db.KaraokeEvents.FindAsync(id);
        if (e == null) return (null, "NotFound");

        if (e.EndTime <= now) return (null, "EndTimePassed");

        e.IsActive = isActive;
        await _db.SaveChangesAsync();

        var model = new KaraokeEvent
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            CreatedByUserId = e.CreatedByUserId,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        };

        return (model, null);
    }

    public async Task<(bool ok, string? error)> EnterKaraokeEventAsync(int eventId, int userId)
    {
        var eventExists = await _db.KaraokeEvents.AnyAsync(e => e.Id == eventId);
        if (!eventExists) return (false, "EventNotFound");

        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return (false, "UserNotFound");

        var alreadyParticipant = await _db.EventParticipants
            .AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        if (alreadyParticipant) return (false, "AlreadyParticipant");

        var participant = new EventParticipantDB
        {
            EventId = eventId,
            UserId = userId,
            Role = ParticipantRole.Participant,
            Status = ParticipantStatus.Invited,
            CreatedAt = DateTime.UtcNow
        };

        _db.EventParticipants.Add(participant);
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool ok, string? error)> RespondInvitationAsync(int eventId, int hostUserId, int userId, ParticipantStatus newStatus)
    {
        var ev = await _db.KaraokeEvents
            .FirstOrDefaultAsync(e => e.Id == eventId && e.CreatedByUserId == hostUserId);
        if (ev == null) return (false, "EventNotFound");

        var participant = await _db.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        if (participant == null) return (false, "InvitationNotFound");

        if (participant.Status != ParticipantStatus.Invited) return (false, "InvitationNotPending");

        participant.Status = newStatus;

        if (newStatus == ParticipantStatus.Accepted)
        {
            await CopyPreferredSongsToSongProposalsAsync(eventId, userId);
        }

        await _db.SaveChangesAsync();

        return (true, null);
    }

    private async Task CopyPreferredSongsToSongProposalsAsync(int eventId, int userId)
    {
        var preferredSongIds = await _db.UserPreferredSongs
            .AsNoTracking()
            .Where(ps => ps.UserId == userId)
            .Select(ps => ps.SongId)
            .ToListAsync();

        if (preferredSongIds.Count == 0) return;

        foreach (var songId in preferredSongIds)
        {
            _db.SongProposals.Add(new SongProposalDB
            {
                EventId = eventId,
                UserId = userId,
                SongId = songId,
                Order = 0,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
