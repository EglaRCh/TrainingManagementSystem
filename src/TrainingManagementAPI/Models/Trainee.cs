using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrainingManagementAPI.Models
{
    public class Trainee
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; } = null!;

        [StringLength(20)]
        public string? Sex { get; set; }          // opcional: "F", "M", etc.

        public DateTime? BirthDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? HeightCm { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? WeightKg { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 1 : N  (un trainee puede tener varios objetivos en el tiempo)
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();

        // 1:N (un trainee puede tener varias evaluaciones)
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    
    }
}
