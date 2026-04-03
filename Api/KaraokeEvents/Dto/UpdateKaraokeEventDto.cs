using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.KaraokeEvents.Dto;

public class UpdateKaraokeEventDto : IValidatableObject
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(300)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int CreatedByUserId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime != default && EndTime != default && StartTime >= EndTime)
            yield return new ValidationResult("StartTime must be before EndTime.", new[] { nameof(StartTime), nameof(EndTime) });
    }
}
