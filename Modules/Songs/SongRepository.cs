using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Songs.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.Songs;

public class SongRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<IEnumerable<SongModel>> GetAllAsync()
    {
        return await _db.Songs
            .AsNoTracking()
            .Select(s => new SongModel
            {
                Id = s.Id,
                ExternalId = s.ExternalId,
                Title = s.Title,
                Artist = s.Artist
            })
            .ToListAsync();
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
