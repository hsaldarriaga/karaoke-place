using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.KaraokeEvents.Dto;

public class GetSongProposalsQueryDto : IValidatableObject
{
    [Range(1, 100, ErrorMessage = "LimitPerEvent must be between 1 and 100.")]
    public int LimitPerEvent { get; set; } = 20;

    public int[] EventIds { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EventIds == null || EventIds.Length == 0)
        {
            yield return new ValidationResult(
                "Provide at least one eventId query parameter.",
                [nameof(EventIds)]);
        }

        if (EventIds.Any(id => id < 1))
        {
            yield return new ValidationResult(
                "All eventIds must be positive integers.",
                [nameof(EventIds)]);
        }
    }
}
