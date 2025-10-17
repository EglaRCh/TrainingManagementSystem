using System.ComponentModel.DataAnnotations;

namespace TrainingManagementAPI.DTOs
{
    // Lo que el cliente ENVÍA para CREAR una evaluación
    public record EvaluationCreateDto(
        [Required] int TraineeId,
        [Required, DataType(DataType.Date)] DateTime Date,
        [Range(0, 500)] decimal? WaistCm,
        [Range(0, 100)] decimal? ArmCm,
        [Range(0, 500)] decimal? WeightKg,
        [Range(0, 75)]  decimal? BodyFatPct,
        [MaxLength(500)] string? Notes
    );

    // Lo que el cliente ENVÍA para ACTUALIZAR una evaluación
    public record EvaluationUpdateDto(
        [Required, DataType(DataType.Date)] DateTime Date,
        [Range(0, 500)] decimal? WaistCm,
        [Range(0, 100)] decimal? ArmCm,
        [Range(0, 500)] decimal? WeightKg,
        [Range(0, 75)]  decimal? BodyFatPct,
        [MaxLength(500)] string? Notes
    );

    // Lo que la API DEVUELVE (puedes incluir Id/CreatedAt)
    public record EvaluationReadDto(
        int Id,
        int TraineeId,
        DateTime Date,
        decimal? WaistCm,
        decimal? ArmCm,
        decimal? WeightKg,
        decimal? BodyFatPct,
        string? Notes,
        DateTime CreatedAt
    );
}
