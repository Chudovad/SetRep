using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности Set
/// </summary>
public class SetConfiguration : IEntityTypeConfiguration<Set>
{
    public void Configure(EntityTypeBuilder<Set> builder)
    {
        builder.ToTable("Sets");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired();

        builder.Property(s => s.WorkoutExerciseId)
            .IsRequired();

        builder.Property(s => s.SetNumber)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(s => s.Reps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.Weight)
            .HasPrecision(10, 2);

        builder.Property(s => s.DurationSeconds);

        builder.Property(s => s.DistanceMeters)
            .HasPrecision(10, 2);

        builder.Property(s => s.IsCompleted)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.CompletedAt)
            .IsRequired();

        builder.HasIndex(s => s.CompletedAt);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Индексы
        builder.HasIndex(s => new { s.WorkoutExerciseId, s.SetNumber });
    }
}

