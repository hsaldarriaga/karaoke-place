using karaoke_place.Modules.Users.Models;

namespace karaoke_place.Modules.Users;

public class UserService(UserRepository repo)
{
    private readonly UserRepository _repo = repo;

    public Task<int?> GetIdByAuth0SubjectAsync(string auth0Subject)
    {
        return _repo.GetIdByAuth0SubjectAsync(auth0Subject);
    }

    public async Task<int?> GetOrCreateByAuth0SubjectAsync(string auth0Subject, string? email)
    {
        var userId = await _repo.GetIdByAuth0SubjectAsync(auth0Subject);
        if (userId != null) return userId;

        var created = await _repo.AddAsync(new UserCreate
        {
            Email = string.IsNullOrWhiteSpace(email)
                ? $"{auth0Subject.Replace("|", ".", StringComparison.Ordinal)}@auth0.local"
                : email,
            Auth0Subject = auth0Subject
        });

        return created.Id;
    }

    public Task<IEnumerable<UserModel>> GetByIdsAsync(IEnumerable<int> userIds)
    {
        return _repo.GetByIdsAsync(userIds);
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
