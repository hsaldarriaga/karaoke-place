using karaoke_place.Modules.Songs.Models;

namespace karaoke_place.Modules.Songs;

public class SongService(SongRepository repo)
{
    private readonly SongRepository _repo = repo;

    public Task<IEnumerable<SongModel>> GetAllAsync()
    {
        return _repo.GetAllAsync();
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
