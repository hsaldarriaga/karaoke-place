using karaoke_place.Api.Songs.Dto;
using karaoke_place.Modules.Songs;
using karaoke_place.Modules.Songs.Models;
using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Api.Songs;

[ApiController]
[Route("api/[controller]")]
public class SongsController(SongService service) : ControllerBase
{
    private readonly SongService _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongModel>>> Get()
    {
        var songs = await _service.GetAllAsync();
        return Ok(songs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SongModel>> Get(int id)
    {
        var song = await _service.GetByIdAsync(id);
        if (song == null) return NotFound();
        return Ok(song);
    }

    [HttpPost]
    public async Task<ActionResult<SongModel>> Create(CreateSongDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new SongCreate
        {
            ExternalId = dto.ExternalId,
            Title = dto.Title,
            Artist = dto.Artist
        };

        var created = await _service.CreateAsync(model);
        return Created($"/api/songs/{created.Id}", created);
    }
}
