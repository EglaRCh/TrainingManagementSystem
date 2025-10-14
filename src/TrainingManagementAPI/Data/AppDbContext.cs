using Microsoft.EntityFrameworkCore;
using TrainingManagementAPI.Models;

namespace TrainingManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Trainee> Trainees => Set<Trainee>();
        public DbSet<Goal> Goals => Set<Goal>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relación 1:N Trainee→Goals
            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Trainee)
                .WithMany(t => t.Goals)
                .HasForeignKey(g => g.TraineeId)
                .OnDelete(DeleteBehavior.Cascade); // si se borra un trainee, se borran sus goals

            modelBuilder.Entity<Goal>()
                .Property(g => g.GoalType)
                .HasConversion<string>();

            // (Opcional) seed mínimo de ejemplo
            modelBuilder.Entity<Trainee>().HasData(
                new Trainee { Id = 1, FullName = "Ana Pérez", Sex = "F", HeightCm = 165, WeightKg = 62, BirthDate = new DateTime(2000, 1, 1), CreatedAt = new DateTime(2025, 2, 10, 0, 0, 0, DateTimeKind.Utc) }
            );
            modelBuilder.Entity<Goal>().HasData(
                new Goal {Id = 1, TraineeId = 1, GoalType = GoalType.PerdidaDeGrasa, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), IsActive = true }
            );
        }
    }
}
