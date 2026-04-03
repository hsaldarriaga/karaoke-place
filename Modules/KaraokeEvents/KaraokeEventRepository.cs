using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.KaraokeEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.KaraokeEvents;

public class KaraokeEventRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<IEnumerable<KaraokeEvent>> GetAllAsync()
    {
        return await _db.KaraokeEvents
            .AsNoTracking()
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
        await _db.SaveChangesAsync();

        return (true, null);
    }
}
