using karaoke_place.Api.Users.Dto;
using karaoke_place.Modules.Auth;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Api.Users;

[ApiController]
[Route("api/[controller]")]
[RequireMockAuth]
public class UsersController(UserService service, CurrentUserContext currentUserContext) : ControllerBase
{
    private readonly UserService _service = service;
    private readonly CurrentUserContext _currentUserContext = currentUserContext;

    [AllowAnonymous]
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

    [HttpPut("me")]
    public async Task<ActionResult> Update(UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = _currentUserContext.GetUserId()!.Value;

        var model = new UserUpdate
        {
            Email = dto.Email
        };

        var ok = await _service.UpdateAsync(userId, model);
        if (!ok) return NotFound();

        return NoContent();
    }

    [HttpPost("me/preferred-songs")]
    public async Task<ActionResult> AddPreferredSong(AddPreferredSongDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = _currentUserContext.GetUserId()!.Value;

        var (ok, error) = await _service.AddPreferredSongAsync(userId, dto.SongId);
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

    [HttpDelete("me/preferred-songs/{songId:int}")]
    public async Task<ActionResult> RemovePreferredSong(int songId)
    {
        var userId = _currentUserContext.GetUserId()!.Value;

        var (ok, error) = await _service.RemovePreferredSongAsync(userId, songId);
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
