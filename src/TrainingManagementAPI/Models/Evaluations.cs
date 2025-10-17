using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementAPI.Models   // 👈 Este namespace debe coincidir con el de Trainee.cs
{
    public class Evaluation
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public DateTime Date { get; set; }
        public decimal? WaistCm { get; set; }
        public decimal? ArmCm { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? BodyFatPct { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación con Trainee
        public Trainee? Trainee { get; set; }
    }
}
