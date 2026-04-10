using karaoke_place.Api.Common;
using karaoke_place.Api.Songs.Dto;
using karaoke_place.Modules.Common;
using karaoke_place.Modules.Songs;
using karaoke_place.Modules.Songs.Models;
using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Api.Songs;

[ApiController]
[Route("api/[controller]")]
public class SongsController(SongService service) : ControllerBase
{
    private readonly SongService _service = service;

    [HttpGet("by-ids")]
    public async Task<ActionResult<IEnumerable<SongModel>>> GetByIds([FromQuery] int[] songIds)
    {
        if (songIds == null || songIds.Length == 0)
        {
            return BadRequest(new
            {
                status = "SONG_IDS_REQUIRED",
                error = "Provide at least one songId query parameter."
            });
        }

        var songs = await _service.GetByIdsAsync(songIds);
        return Ok(songs);
    }

    [HttpGet("by-user/{userId:int}")]
    public async Task<ActionResult<PagedResult<SongModel>>> GetByUserId(int userId, [FromQuery] PaginationParams pagination)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var songs = await _service.GetByUserIdAsync(userId, pagination.Page, pagination.PageSize);
        return Ok(songs);
    }

    [HttpGet("by-event")]
    public async Task<ActionResult<SongsByEventModel>> GetByEventId(int eventId, [FromQuery] PaginationParams pagination)
    {
        if (eventId < 1)
        {
            return BadRequest(new
            {
                status = "EVENT_ID_REQUIRED",
                error = "Provide a valid eventId query parameter."
            });
        }

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var songs = await _service.GetByEventIdAsync(eventId, pagination.Page, pagination.PageSize);
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
