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

    [HttpGet("by-events")]
    public async Task<ActionResult<IEnumerable<SongsByEventModel>>> GetByEventIds([FromQuery] int[] eventIds, [FromQuery] int limit = 20)
    {
        if (eventIds == null || eventIds.Length == 0)
        {
            return BadRequest(new
            {
                status = "EVENT_IDS_REQUIRED",
                error = "Provide at least one eventId query parameter."
            });
        }

        if (limit < 1)
        {
            return BadRequest(new
            {
                status = "INVALID_LIMIT",
                error = "Limit must be greater than 0."
            });
        }

        var songs = await _service.GetByEventIdsAsync(eventIds, limit);
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
