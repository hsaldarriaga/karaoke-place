using karaoke_place.Modules.Users.Models;

namespace karaoke_place.Modules.Users;

public class UserService(UserRepository repo)
{
    private readonly UserRepository _repo = repo;

    public Task<IEnumerable<UserModel>> GetAllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<UserModel?> GetByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }

    public Task<IEnumerable<karaoke_place.Modules.Songs.Models.SongModel>> GetPreferredSongsAsync(int userId)
    {
        return _repo.GetPreferredSongsAsync(userId);
    }

    public Task<UserModel> CreateAsync(UserCreate model)
    {
        return _repo.AddAsync(model);
    }

    public Task<bool> UpdateAsync(int id, UserUpdate model)
    {
        return _repo.UpdateAsync(id, model);
    }

    public Task<(bool ok, string? error)> AddPreferredSongAsync(int userId, int songId)
    {
        return _repo.AddPreferredSongAsync(userId, songId);
    }

    public Task<(bool ok, string? error)> RemovePreferredSongAsync(int userId, int songId)
    {
        return _repo.RemovePreferredSongAsync(userId, songId);
    }
}
