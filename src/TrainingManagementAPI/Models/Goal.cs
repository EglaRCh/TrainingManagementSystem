using System.ComponentModel.DataAnnotations;

namespace TrainingManagementAPI.Models
{
    public enum GoalType
    {
        Hipertrofia,
        PerdidaDeGrasa,
        Resistencia,
        Fuerza,
    }

    public class Goal
    {
        [Key] // ✅ clave primaria
        public int Id { get; set; }
        public int TraineeId { get; set; }

         [Required]
        public GoalType GoalType { get; set; } // ← aquí usamos el enum definido arriba

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? TargetNote { get; set; }   // p. ej. "−5 kg en 12 semanas"

        public bool IsActive { get; set; } = true;

        // navegación
        public Trainee? Trainee { get; set; }

        // 1:N - Un goal tiene muchos módulos
        public ICollection<Module> Modules { get; set; } = new List<Module>();

    }
}
// Nota: no es necesario un Id autonumérico porque la PK es (TraineeId, StartDate)
