using karaoke_place.Data;
using karaoke_place.Models;
using karaoke_place.Modules.Songs.Models;
using karaoke_place.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace karaoke_place.Modules.Users;

public class UserRepository(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public Task<int?> GetIdByAuth0SubjectAsync(string auth0Subject)
    {
        return _db.Users
            .AsNoTracking()
            .Where(u => u.Auth0Subject == auth0Subject)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserModel>> GetByIdsAsync(IEnumerable<int> userIds)
    {
        var normalizedUserIds = userIds.Distinct().ToArray();

        return await _db.Users
            .AsNoTracking()
            .Where(u => normalizedUserIds.Contains(u.Id))
            .OrderBy(u => u.Id)
            .Select(u => new UserModel
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
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
                FirstName = u.FirstName,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SongModel>> GetPreferredSongsAsync(int userId)
    {
        return await _db.UserPreferredSongs
            .AsNoTracking()
            .Where(ps => ps.UserId == userId)
            .OrderBy(ps => ps.Sort)
            .ThenBy(ps => ps.CreatedAt)
            .Select(ps => new SongModel
            {
                Id = ps.Song.Id,
                ExternalId = ps.Song.ExternalId,
                Title = ps.Song.Title,
                Artist = ps.Song.Artist,
                Order = ps.Sort
            })
            .ToListAsync();
    }

    public async Task<UserModel> AddAsync(UserCreate model)
    {
        if (!string.IsNullOrWhiteSpace(model.Auth0Subject))
        {
            var existingUser = await _db.Users
                .AsNoTracking()
                .Where(u => u.Auth0Subject == model.Auth0Subject)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (existingUser != null)
                return existingUser;
        }

        var user = new UserDB
        {
            Email = model.Email,
            FirstName = model.FirstName,
            Auth0Subject = model.Auth0Subject,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, UserUpdate model)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return false;

        user.Email = model.Email;
        user.FirstName = model.FirstName ?? string.Empty;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool ok, string? error)> AddPreferredSongAsync(int userId, string externalId, string title, string artist)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return (false, "UserNotFound");

        var song = await _db.Songs.FirstOrDefaultAsync(s => s.ExternalId == externalId);
        if (song == null)
        {
            song = new SongDB
            {
                ExternalId = externalId,
                Title = title,
                Artist = artist
            };
            _db.Songs.Add(song);
            await _db.SaveChangesAsync();
        }

        var alreadyPreferred = await _db.UserPreferredSongs
            .AnyAsync(ps => ps.UserId == userId && ps.SongId == song.Id);
        if (alreadyPreferred) return (false, "AlreadyPreferred");

        var nextSort = (await _db.UserPreferredSongs
            .Where(ps => ps.UserId == userId)
            .MaxAsync(ps => (int?)ps.Sort) ?? 0) + 1;

        var preferredSong = new UserPreferredSongDB
        {
            UserId = userId,
            SongId = song.Id,
            Sort = nextSort,
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
