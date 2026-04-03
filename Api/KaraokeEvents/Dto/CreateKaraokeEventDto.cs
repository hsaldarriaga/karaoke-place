using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace karaoke_place.Api.KaraokeEvents.Dto;

public class CreateKaraokeEventDto : IValidatableObject
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

    public bool? IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime == default)
            yield return new ValidationResult("StartTime is required.", new[] { nameof(StartTime) });

        if (EndTime == default)
            yield return new ValidationResult("EndTime is required.", new[] { nameof(EndTime) });

        if (StartTime != default && EndTime != default && StartTime >= EndTime)
            yield return new ValidationResult("StartTime must be before EndTime.", new[] { nameof(StartTime), nameof(EndTime) });

        if (CreatedByUserId <= 0)
            yield return new ValidationResult("CreatedByUserId must be a positive integer.", new[] { nameof(CreatedByUserId) });
    }
}
