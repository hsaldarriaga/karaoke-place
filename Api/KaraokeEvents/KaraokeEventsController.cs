using Microsoft.AspNetCore.Mvc;
using karaoke_place.Api.Common;
using karaoke_place.Modules.Common;
using karaoke_place.Modules.KaraokeEvents;
using karaoke_place.Api.KaraokeEvents.Dto;
using karaoke_place.Modules.KaraokeEvents.Models;

namespace karaoke_place.Api.KaraokeEvents;

[ApiController]
[Route("api/[controller]")]
public class KaraokeEventsController(KaraokeEventService service) : ControllerBase
{
    private readonly KaraokeEventService _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResult<KaraokeEvent>>> Get(
        [FromQuery] bool? isActive,
        [FromQuery] PaginationParams pagination)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.GetAllAsync(isActive, pagination.Page, pagination.PageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<KaraokeEvent>> Get(int id)
    {
        var ev = await _service.GetByIdAsync(id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpGet("participants")]
    public async Task<ActionResult<IEnumerable<EventParticipantsByEventModel>>> GetParticipants([FromQuery] int[] eventIds)
    {
        if (eventIds == null || eventIds.Length == 0)
            return BadRequest(new { status = "EVENT_IDS_REQUIRED", error = "Provide at least one eventId query parameter." });

        var participants = await _service.GetParticipantsAsync(eventIds);
        return Ok(participants);
    }

    [HttpGet("songProposals")]
    public async Task<ActionResult<IEnumerable<SongProposalsByEventModel>>> GetSongProposals([FromQuery] GetSongProposalsQueryDto query)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var proposals = await _service.GetSongProposalsAsync(query.EventIds, query.LimitPerEvent);
        return Ok(proposals);
    }

    [HttpPost]
    public async Task<ActionResult<KaraokeEvent>> Create(CreateKaraokeEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new KaraokeEventCreate
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            CreatedByUserId = dto.CreatedByUserId,
        };

        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateKaraokeEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var model = new KaraokeEventUpdate
        {
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            CreatedByUserId = dto.CreatedByUserId,
        };

        var ok = await _service.UpdateAsync(id, model);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:int}/publish")]
    public async Task<ActionResult> Publish(int id)
    {
        var (ok, error) = await _service.PublishAsync(id);
        if (!ok)
        {
            if (error == "NotFound")
                return NotFound(new { status = "NOT_FOUND", error = "Event not found." });
            if (error == "EndTimePassed")
                return BadRequest(new { status = "END_TIME_PASSED", error = "Cannot publish event: EndTime has already passed." });
            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }

    [HttpPost("{id:int}/enterKaraokeEvent")]
    public async Task<ActionResult> EnterKaraokeEvent(int id, EnterKaraokeEventDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (ok, error) = await _service.EnterKaraokeEventAsync(id, dto.UserId);
        if (!ok)
        {
            if (error == "EventNotFound")
                return NotFound(new { status = "EVENT_NOT_FOUND", error = "Event not found." });
            if (error == "UserNotFound")
                return NotFound(new { status = "USER_NOT_FOUND", error = "User not found." });
            if (error == "AlreadyParticipant")
                return Conflict(new { status = "ALREADY_PARTICIPANT", error = "User is already added to this event." });

            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }

    [HttpPost("{id:int}/acceptInvitation")]
    public async Task<ActionResult> AcceptInvitation(int id, RespondInvitationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (ok, error) = await _service.AcceptInvitationAsync(id, dto.HostUserId, dto.UserId);
        if (!ok)
        {
            if (error == "EventNotFound")
                return NotFound(new { status = "EVENT_NOT_FOUND", error = "Event not found." });
            if (error == "InvitationNotFound")
                return NotFound(new { status = "INVITATION_NOT_FOUND", error = "Invitation not found." });
            if (error == "InvitationNotPending")
                return Conflict(new { status = "INVITATION_NOT_PENDING", error = "Invitation is not pending." });

            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }

    [HttpPost("{id:int}/rejectInvitation")]
    public async Task<ActionResult> RejectInvitation(int id, RespondInvitationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (ok, error) = await _service.RejectInvitationAsync(id, dto.HostUserId, dto.UserId);
        if (!ok)
        {
            if (error == "EventNotFound")
                return NotFound(new { status = "EVENT_NOT_FOUND", error = "Event not found." });
            if (error == "InvitationNotFound")
                return NotFound(new { status = "INVITATION_NOT_FOUND", error = "Invitation not found." });
            if (error == "InvitationNotPending")
                return Conflict(new { status = "INVITATION_NOT_PENDING", error = "Invitation is not pending." });

            return BadRequest(new { status = "ERROR", error = error });
        }

        return NoContent();
    }
}
