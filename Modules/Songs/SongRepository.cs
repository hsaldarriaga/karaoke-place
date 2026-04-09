using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Common;
using karaoke_place.Modules.Songs.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.Songs;

public class SongRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<IEnumerable<SongModel>> GetByIdsAsync(IEnumerable<int> songIds)
    {
        var normalizedSongIds = songIds.Distinct().ToArray();

        return await _db.Songs
            .AsNoTracking()
            .Where(s => normalizedSongIds.Contains(s.Id))
            .OrderBy(s => s.Title)
            .ThenBy(s => s.Artist)
            .ThenBy(s => s.Id)
            .Select(s => new SongModel
            {
                Id = s.Id,
                ExternalId = s.ExternalId,
                Title = s.Title,
                Artist = s.Artist
            })
            .ToListAsync();
    }

    public async Task<PagedResult<SongModel>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20)
    {
        var query = _db.Songs
            .AsNoTracking()
            .Where(s => _db.UserPreferredSongs.Any(ps => ps.UserId == userId && ps.SongId == s.Id))
            .OrderBy(s => s.Title)
            .ThenBy(s => s.Artist)
            .ThenBy(s => s.Id);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SongModel
            {
                Id = s.Id,
                ExternalId = s.ExternalId,
                Title = s.Title,
                Artist = s.Artist
            })
            .ToListAsync();

        return new PagedResult<SongModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<IEnumerable<SongsByEventModel>> GetByEventIdsAsync(IEnumerable<int> eventIds, int limit)
    {
        var eventIdList = eventIds.Distinct().ToList();

        var proposedSongs = await _db.SongProposals
            .AsNoTracking()
            .Where(sp => eventIdList.Contains(sp.EventId))
            .OrderBy(sp => sp.EventId)
            .ThenBy(sp => sp.Order)
            .ThenBy(sp => sp.CreatedAt)
            .Select(sp => new
            {
                sp.EventId,
                Song = new SongModel
                {
                    Id = sp.Song.Id,
                    ExternalId = sp.Song.ExternalId,
                    Title = sp.Song.Title,
                    Artist = sp.Song.Artist
                }
            })
            .ToListAsync();

        var songsByEventId = proposedSongs
            .GroupBy(x => x.EventId)
            .ToDictionary(
                group => group.Key,
                group => (IEnumerable<SongModel>)group
                    .GroupBy(x => x.Song.Id)
                    .Select(songGroup => songGroup.First().Song)
                    .Take(limit)
                    .ToList());

        return eventIdList.Select(eventId => new SongsByEventModel
        {
            EventId = eventId,
            Songs = songsByEventId.GetValueOrDefault(eventId, [])
        });
    }

    public async Task<SongModel?> GetByIdAsync(int id)
    {
        return await _db.Songs
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SongModel
            {
                Id = s.Id,
                ExternalId = s.ExternalId,
                Title = s.Title,
                Artist = s.Artist
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SongModel> AddAsync(SongCreate model)
    {
        var song = new SongDB
        {
            ExternalId = model.ExternalId,
            Title = model.Title,
            Artist = model.Artist
        };

        _db.Songs.Add(song);
        await _db.SaveChangesAsync();

        return new SongModel
        {
            Id = song.Id,
            ExternalId = song.ExternalId,
            Title = song.Title,
            Artist = song.Artist
        };
    }
}
