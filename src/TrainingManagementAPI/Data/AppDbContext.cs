using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Models;

namespace TrainingManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- Tablas principales ---
        public DbSet<Trainee> Trainees => Set<Trainee>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<Evaluation> Evaluations => Set<Evaluation>();
        public DbSet<Module> Modules => Set<Module>();

        // --- Configuración del modelo (Fluent API) ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ------------------------------
            // 1️⃣ Relación 1:N Trainee → Goals
            // ------------------------------
            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Trainee)
                .WithMany(t => t.Goals)
                .HasForeignKey(g => g.TraineeId)
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el Trainee, se borran sus Goals

            // Conversion enum→string (GoalType)
            modelBuilder.Entity<Goal>()
                .Property(g => g.GoalType)
                .HasConversion<string>();

            // ------------------------------
            // 2️⃣ Relación 1:N Trainee → Evaluations
            // ------------------------------
            modelBuilder.Entity<Evaluation>(e =>
            {
                // Reglas de precisión para valores decimales
                e.Property(p => p.WaistCm).HasPrecision(5, 2);
                e.Property(p => p.ArmCm).HasPrecision(5, 2);
                e.Property(p => p.WeightKg).HasPrecision(5, 2);
                e.Property(p => p.BodyFatPct).HasPrecision(5, 2);

                // Longitud máxima de notas
                e.Property(p => p.Notes).HasMaxLength(500);

                // Relación con Trainee (1:N)
                e.HasOne(p => p.Trainee)
                 .WithMany(t => t.Evaluations)
                 .HasForeignKey(p => p.TraineeId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Índice compuesto para optimizar búsqueda por Trainee y Fecha
                e.HasIndex(p => new { p.TraineeId, p.Date });
            });

            // ------------------------------
            // 3️⃣ Relación 1:N Goal → Modules
            // ------------------------------
            modelBuilder.Entity<Module>(m =>
            {
                // Tipo de módulo: requerido, máx. 40 caracteres
                m.Property(p => p.Type)
                 .HasMaxLength(40)
                 .IsRequired();

                // Notas opcionales
                m.Property(p => p.Notes)
                 .HasMaxLength(500);

                // Relación con Goal
                m.HasOne(p => p.Goal)
                 .WithMany(g => g.Modules)
                 .HasForeignKey(p => p.GoalId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Índice simple en GoalId
                m.HasIndex(p => p.GoalId);
            });

            // ------------------------------
            // 4️⃣ (Opcional) Datos semilla iniciales
            // ------------------------------
            modelBuilder.Entity<Trainee>().HasData(
                new Trainee
                {
                    Id = 1,
                    FullName = "Ana Pérez",
                    Sex = "F",
                    HeightCm = 165,
                    WeightKg = 62,
                    BirthDate = new DateTime(2000, 1, 1),
                    CreatedAt = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Goal>().HasData(
                new Goal
                {
                    Id = 1,
                    TraineeId = 1,
                    GoalType = GoalType.PerdidaDeGrasa,
                    StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );

            // **Fin de la configuración**
        }
    }
}

