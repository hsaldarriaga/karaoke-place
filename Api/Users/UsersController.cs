using karaoke_place.Api.Users.Dto;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Api.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService service) : ControllerBase
{
    private readonly UserService _service = service;

    [HttpPost]
    public async Task<ActionResult<UserModel>> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new UserCreate
        {
            Email = dto.Email
        };

        var created = await _service.CreateAsync(model);
        return Created($"/api/users/{created.Id}", created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new UserUpdate
        {
            Email = dto.Email
        };

        var ok = await _service.UpdateAsync(id, model);
        if (!ok) return NotFound();

        return NoContent();
    }

    [HttpPost("{id:int}/preferred-songs")]
    public async Task<ActionResult> AddPreferredSong(int id, AddPreferredSongDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (ok, error) = await _service.AddPreferredSongAsync(id, dto.SongId);
        if (!ok)
        {
            if (error == "UserNotFound")
                return NotFound(new { status = "USER_NOT_FOUND", error = "User not found." });
            if (error == "SongNotFound")
                return NotFound(new { status = "SONG_NOT_FOUND", error = "Song not found." });
            if (error == "AlreadyPreferred")
                return Conflict(new { status = "ALREADY_PREFERRED", error = "Song is already in user's preferred songs." });

            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}/preferred-songs/{songId:int}")]
    public async Task<ActionResult> RemovePreferredSong(int id, int songId)
    {
        var (ok, error) = await _service.RemovePreferredSongAsync(id, songId);
        if (!ok)
        {
            if (error == "UserNotFound")
                return NotFound(new { status = "USER_NOT_FOUND", error = "User not found." });
            if (error == "PreferredSongNotFound")
                return NotFound(new { status = "PREFERRED_SONG_NOT_FOUND", error = "Preferred song not found for user." });

            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }
}
