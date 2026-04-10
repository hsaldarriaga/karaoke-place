using karaoke_place.Modules.Common;
using karaoke_place.Modules.Songs.Models;

namespace karaoke_place.Modules.Songs;

public class SongService(SongRepository repo)
{
    private readonly SongRepository _repo = repo;

    public Task<IEnumerable<SongModel>> GetByIdsAsync(IEnumerable<int> songIds)
    {
        return _repo.GetByIdsAsync(songIds);
    }

    public Task<PagedResult<SongModel>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20)
    {
        return _repo.GetByUserIdAsync(userId, page, pageSize);
    }

    public Task<SongsByEventModel> GetByEventIdAsync(int eventId, int page = 1, int pageSize = 20)
    {
        return _repo.GetByEventIdAsync(eventId, page, pageSize);
    }

    public Task<SongModel?> GetByIdAsync(int id)
    {
        return _repo.GetByIdAsync(id);
    }

    public Task<SongModel> CreateAsync(SongCreate model)
    {
        return _repo.AddAsync(model);
    }
}
