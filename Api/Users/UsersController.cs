using karaoke_place.Api.Common;
using karaoke_place.Api.Users.Dto;
using karaoke_place.Modules.Songs.Models;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace karaoke_place.Api.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(UserService service, CurrentUserContext currentUserContext) : ControllerBase
{
    private readonly UserService _service = service;
    private readonly CurrentUserContext _currentUserContext = currentUserContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserModel>>> Get([FromQuery] int[] userIds)
    {
        if (userIds == null || userIds.Length == 0)
        {
            return BadRequest(new
            {
                status = "USER_IDS_REQUIRED",
                error = "Provide at least one userId query parameter."
            });
        }

        var users = await _service.GetByIdsAsync(userIds);
        return Ok(users);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserModel>> GetCurrentUser()
    {
        var userId = await _currentUserContext.GetUserIdAsync();
        if (userId == null)
            return Unauthorized(new { status = "USER_NOT_LINKED", error = "Authenticated user is not linked to a local user record." });

        var user = await _service.GetByIdAsync(userId.Value);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserModel>> Get(int id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/preferred-songs")]
    public async Task<ActionResult<IEnumerable<SongModel>>> GetPreferredSongs(int id)
    {
        var user = await _service.GetByIdAsync(id);
        if (user == null) return NotFound();

        var songs = await _service.GetPreferredSongsAsync(id);
        return Ok(songs);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<UserModel>> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new UserCreate
        {
            Email = dto.Email,
            Auth0Subject = _currentUserContext.GetSubject()
        };

        var created = await _service.CreateAsync(model);
        return Created($"/api/users/{created.Id}", created);
    }

    [HttpPut("me")]
    public async Task<ActionResult> Update(UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = await _currentUserContext.GetUserIdAsync();
        if (userId == null)
            return Unauthorized(new { status = "USER_NOT_LINKED", error = "Authenticated user is not linked to a local user record." });

        var model = new UserUpdate
        {
            Email = dto.Email
        };

        var ok = await _service.UpdateAsync(userId.Value, model);
        if (!ok) return NotFound();

        return NoContent();
    }

    [HttpPost("me/preferred-songs")]
    public async Task<ActionResult> AddPreferredSong(AddPreferredSongDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = await _currentUserContext.GetUserIdAsync();
        if (userId == null)
            return Unauthorized(new { status = "USER_NOT_LINKED", error = "Authenticated user is not linked to a local user record." });

        var (ok, error) = await _service.AddPreferredSongAsync(userId.Value, dto.SongId);
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
        var userId = await _currentUserContext.GetUserIdAsync();
        if (userId == null)
            return Unauthorized(new { status = "USER_NOT_LINKED", error = "Authenticated user is not linked to a local user record." });

        var (ok, error) = await _service.RemovePreferredSongAsync(userId.Value, songId);
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
