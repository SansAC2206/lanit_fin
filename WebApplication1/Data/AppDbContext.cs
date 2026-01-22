using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TrainingProgram> TrainingPrograms { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrainingProgram>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.TrainingProgram)
                    .WithMany()
                    .HasForeignKey(e => e.TrainingProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("Activities", t => t.HasTrigger("SomeTrigger"));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.Minutes)
                    .IsRequired();

                entity.Property(e => e.Notes)
                    .HasMaxLength(500);

                entity.Property(e => e.IsExerciseActiveAtCreation)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.HasOne(a => a.Exercise)
                    .WithMany()
                    .HasForeignKey(a => a.ExerciseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
