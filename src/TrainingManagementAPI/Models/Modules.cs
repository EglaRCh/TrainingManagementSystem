using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingManagementAPI.Models   // 👈 Este namespace debe coincidir con el de Trainee.cs
{
    public class Module

    {
        public int Id { get; set; }
        public int GoalId { get; set; }
        public string Type { get; set; } = default!;  // e.g. "Hipertrofia"
        public int DurationWeeks { get; set; }
        public int SessionsPerWeek { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Goal? Goal { get; set; }
    }
}
