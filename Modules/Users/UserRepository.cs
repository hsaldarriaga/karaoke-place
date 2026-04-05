using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Songs.Models;
using karaoke_place.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.Users;

public class UserRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        return await _db.Users
            .AsNoTracking()
            .Select(u => new UserModel
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserModel?> GetByIdAsync(int id)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserModel
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SongModel>> GetPreferredSongsAsync(int userId)
    {
        return await _db.UserPreferredSongs
            .AsNoTracking()
            .Where(ps => ps.UserId == userId)
            .OrderBy(ps => ps.CreatedAt)
            .Select(ps => new SongModel
            {
                Id = ps.Song.Id,
                ExternalId = ps.Song.ExternalId,
                Title = ps.Song.Title,
                Artist = ps.Song.Artist
            })
            .ToListAsync();
    }

    public async Task<UserModel> AddAsync(UserCreate model)
    {
        var user = new UserDB
        {
            Email = model.Email,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, UserUpdate model)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return false;

        user.Email = model.Email;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool ok, string? error)> AddPreferredSongAsync(int userId, int songId)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return (false, "UserNotFound");

        var songExists = await _db.Songs.AnyAsync(s => s.Id == songId);
        if (!songExists) return (false, "SongNotFound");

        var alreadyPreferred = await _db.UserPreferredSongs
            .AnyAsync(ps => ps.UserId == userId && ps.SongId == songId);
        if (alreadyPreferred) return (false, "AlreadyPreferred");

        var preferredSong = new UserPreferredSongDB
        {
            UserId = userId,
            SongId = songId,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserPreferredSongs.Add(preferredSong);
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool ok, string? error)> RemovePreferredSongAsync(int userId, int songId)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return (false, "UserNotFound");

        var preferredSong = await _db.UserPreferredSongs
            .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.SongId == songId);
        if (preferredSong == null) return (false, "PreferredSongNotFound");

        _db.UserPreferredSongs.Remove(preferredSong);
        await _db.SaveChangesAsync();

        return (true, null);
    }
}
