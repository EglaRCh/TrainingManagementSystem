using System.ComponentModel.DataAnnotations;

namespace TrainingManagementAPI.DTOs
{
    public record ModuleCreateDto(
        [Required] int GoalId,
        [Required, StringLength(40)] string Type,
        [Range(1, 52)] int DurationWeeks,
        [Range(1, 14)] int SessionsPerWeek,
        [MaxLength(500)] string? Notes
    );

    public record ModuleUpdateDto(
        [Required, StringLength(40)] string Type,
        [Range(1, 52)] int DurationWeeks,
        [Range(1, 14)] int SessionsPerWeek,
        [MaxLength(500)] string? Notes
    );

    public record ModuleReadDto(
        int Id,
        int GoalId,
        string Type,
        int DurationWeeks,
        int SessionsPerWeek,
        string? Notes,
        DateTime CreatedAt
    );
}
