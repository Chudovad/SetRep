using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности Exercise
/// </summary>
public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.Name);

        builder.Property(e => e.Image)
            .HasMaxLength(2000);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.MuscleGroup)
            .HasConversion<int>()
            .HasComment("Группа мышц");

        builder.Property(e => e.WeightUnit)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("кг");

        builder.Property(e => e.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedByUserId);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Индексы
        builder.HasIndex(e => new { e.IsSystem, e.CreatedByUserId });
        builder.HasIndex(e => e.MuscleGroup);

        // Связи
        builder.HasMany(e => e.WorkoutExercises)
            .WithOne(we => we.Exercise)
            .HasForeignKey(we => we.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

