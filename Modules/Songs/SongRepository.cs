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

    public async Task<SongsByEventModel> GetByEventIdAsync(int eventId, int page = 1, int pageSize = 20)
    {
        var proposedSongs = await _db.SongProposals
            .AsNoTracking()
            .Where(sp => sp.EventId == eventId)
            .OrderBy(sp => sp.Order)
            .ThenBy(sp => sp.CreatedAt)
            .ThenBy(sp => sp.Id)
            .Select(sp => new SongByEventModel
            {
                Id = sp.Song.Id,
                UserId = sp.UserId,
                ExternalId = sp.Song.ExternalId,
                Title = sp.Song.Title,
                Artist = sp.Song.Artist
            })
            .ToListAsync();

        var uniqueSongs = proposedSongs
            .GroupBy(song => song.Id)
            .Select(songGroup => songGroup.First())
            .ToList();

        var totalCount = uniqueSongs.Count;
        var items = uniqueSongs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new SongsByEventModel
        {
            EventId = eventId,
            Songs = new PagedResult<SongByEventModel>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            }
        };
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
