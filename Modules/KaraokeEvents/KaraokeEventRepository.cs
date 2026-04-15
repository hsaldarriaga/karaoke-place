using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Common;
using karaoke_place.Modules.KaraokeEvents.Models;
using karaoke_place.Modules.Songs.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace karaoke_place.Modules.KaraokeEvents;

public class KaraokeEventRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<PagedResult<KaraokeEvent>> GetAllAsync(bool? isActive = null, int? createdByUserId = null, int? participantUserId = null, int page = 1, int pageSize = 20)
    {
        var query = _db.KaraokeEvents.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(e => e.IsActive == isActive.Value);
        }

        if (createdByUserId.HasValue)
        {
            query = query.Where(e => e.CreatedByUserId == createdByUserId.Value);
        }

        if (participantUserId.HasValue)
        {
            query = query.Where(e => e.CreatedByUserId != participantUserId.Value &&
                e.Participants.Any(p => p.UserId == participantUserId.Value));
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
                Coordinates = e.Coordinates,
                StartTime = e.StartTime,
                Hours = e.Hours,
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
            Coordinates = e.Coordinates,
            StartTime = e.StartTime,
            Hours = e.Hours,
            CreatedByUserId = e.CreatedByUserId,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        };
    }

    public async Task<IEnumerable<ParticipantCountByEventModel>> GetParticipantCountsAsync(IEnumerable<int> eventIds)
    {
        var eventIdList = eventIds.Distinct().ToList();

        var counts = await _db.EventParticipants
            .AsNoTracking()
            .Where(ep => eventIdList.Contains(ep.EventId))
            .GroupBy(ep => ep.EventId)
            .Select(g => new ParticipantCountByEventModel
            {
                EventId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var countsByEventId = counts.ToDictionary(c => c.EventId);

        return eventIdList.Select(eventId => countsByEventId.TryGetValue(eventId, out var c)
            ? c
            : new ParticipantCountByEventModel { EventId = eventId, Count = 0 });
    }

    public async Task<bool> IsUserAuthorizedForEventsAsync(IEnumerable<int> eventIds, int userId)
    {
        var eventIdList = eventIds.Distinct().ToList();

        var authorizedCount = await _db.KaraokeEvents
            .AsNoTracking()
            .Where(e => eventIdList.Contains(e.Id) && (
                e.CreatedByUserId == userId ||
                e.Participants.Any(p => p.UserId == userId)
            ))
            .CountAsync();

        return authorizedCount == eventIdList.Count;
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
            .ToDictionary(group => group.Key, group => (IEnumerable<EventParticipantModel>)[.. group]);

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

        var eventIdParams = string.Join(",", eventIdList.Select((id, i) => $"@p{i}"));
        var parameters = eventIdList.Select((id, i) => new NpgsqlParameter($"p{i}", id)).ToArray();

        var sql = $@"
            SELECT sp.""Id"", sp.""EventId"", sp.""UserId"", sp.""SongId"", sp.""Order"", sp.""CreatedAt"",
                s.""Id"" AS ""Song_Id"", s.""ExternalId"", s.""Title"", s.""Artist""
            FROM (
                SELECT *, ROW_NUMBER() OVER (PARTITION BY ""EventId"" ORDER BY ""Order"", ""CreatedAt"", ""Id"") AS rn
                FROM ""SongProposals""
                WHERE ""EventId"" IN ({eventIdParams})
            ) sp
            JOIN ""Songs"" s ON sp.""SongId"" = s.""Id""
            WHERE sp.rn <= @limitPerEvent
            ORDER BY sp.""EventId"", sp.""Order"", sp.""CreatedAt"", sp.""Id"";
        ";

        var limitParam = new NpgsqlParameter("limitPerEvent", limitPerEvent);
        var allParams = parameters.Concat(new[] { limitParam }).ToArray();

        var rawResults = new List<RawSongProposalRow>();
        using (var conn = _db.Database.GetDbConnection())
        {
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(allParams);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rawResults.Add(new RawSongProposalRow
                {
                    Id = reader.GetInt32(0),
                    EventId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    SongId = reader.GetInt32(3),
                    Order = reader.GetInt32(4),
                    CreatedAt = reader.GetDateTime(5),
                    Song_Id = reader.GetInt32(6),
                    ExternalId = reader.GetString(7),
                    Title = reader.GetString(8),
                    Artist = reader.GetString(9)
                });
            }
        }

        var proposals = rawResults.Select(r => new SongProposalModel
        {
            Id = r.Id,
            EventId = r.EventId,
            UserId = r.UserId,
            SongId = r.SongId,
            Order = r.Order,
            CreatedAt = r.CreatedAt,
            Song = new SongModel
            {
                Id = r.Song_Id,
                ExternalId = r.ExternalId,
                Title = r.Title,
                Artist = r.Artist
            }
        }).ToList();

        var grouped = proposals
            .GroupBy(sp => sp.EventId)
            .ToDictionary(g => g.Key, g => (IEnumerable<SongProposalModel>)[.. g]);

        return eventIdList.Select(eventId => new SongProposalsByEventModel
        {
            EventId = eventId,
            SongProposals = grouped.GetValueOrDefault(eventId, [])
        });
    }

    public async Task<KaraokeEvent> AddAsync(KaraokeEventCreate model)
    {
        var e = new KaraokeEventDB
        {
            Name = model.Name,
            Description = model.Description ?? string.Empty,
            Location = model.Location,
            Coordinates = model.Coordinates,
            StartTime = model.StartTime,
            Hours = model.Hours,
            CreatedByUserId = model.CreatedByUserId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await using var transaction = await _db.Database.BeginTransactionAsync();

        _db.KaraokeEvents.Add(e);
        await _db.SaveChangesAsync();

        await CopyPreferredSongsToSongProposalsAsync(e.Id, e.CreatedByUserId);
        await _db.SaveChangesAsync();

        await transaction.CommitAsync();

        return new KaraokeEvent
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            Coordinates = e.Coordinates,
            StartTime = e.StartTime,
            Hours = e.Hours,
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
        e.Coordinates = model.Coordinates;
        e.StartTime = model.StartTime;
        e.Hours = model.Hours;
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

        if (e.StartTime.AddHours(e.Hours) <= now) return (null, "EventEnded");

        e.IsActive = isActive;
        await _db.SaveChangesAsync();

        var model = new KaraokeEvent
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Location = e.Location,
            Coordinates = e.Coordinates,
            StartTime = e.StartTime,
            Hours = e.Hours,
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
