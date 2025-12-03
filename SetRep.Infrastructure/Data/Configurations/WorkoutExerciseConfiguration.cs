using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности WorkoutExercise
/// </summary>
public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.ToTable("WorkoutExercises");

        builder.HasKey(we => we.Id);

        builder.Property(we => we.Id)
            .IsRequired();

        builder.Property(we => we.WorkoutId)
            .IsRequired();

        builder.Property(we => we.ExerciseId)
            .IsRequired();

        builder.Property(we => we.Order)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(we => we.Notes)
            .HasMaxLength(2000);

        builder.Property(we => we.CreatedAt)
            .IsRequired();

        // Индексы
        builder.HasIndex(we => new { we.WorkoutId, we.Order });
        builder.HasIndex(we => we.ExerciseId);

        // Уникальный индекс для предотвращения дублирования упражнений в одной тренировке
        builder.HasIndex(we => new { we.WorkoutId, we.ExerciseId })
            .IsUnique();

        // Связи
        builder.HasMany(we => we.Sets)
            .WithOne(s => s.WorkoutExercise)
            .HasForeignKey(s => s.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

