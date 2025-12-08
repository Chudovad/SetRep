using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности WorkoutHistorySet
/// </summary>
public class WorkoutHistorySetConfiguration : IEntityTypeConfiguration<WorkoutHistorySet>
{
    public void Configure(EntityTypeBuilder<WorkoutHistorySet> builder)
    {
        builder.ToTable("WorkoutHistorySets");

        builder.HasKey(whs => whs.Id);

        builder.Property(whs => whs.Id)
            .IsRequired();

        builder.Property(whs => whs.WorkoutHistoryId)
            .IsRequired();

        builder.Property(whs => whs.ExerciseId)
            .IsRequired();

        builder.Property(whs => whs.SetNumber)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(whs => whs.Reps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(whs => whs.Weight)
            .HasPrecision(10, 2);

        builder.Property(whs => whs.DurationSeconds);

        builder.Property(whs => whs.DistanceMeters)
            .HasPrecision(10, 2);

        builder.Property(whs => whs.RestSeconds);

        builder.Property(whs => whs.IsCompleted)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(whs => whs.Notes)
            .HasMaxLength(1000);

        builder.Property(whs => whs.CompletedAt)
            .IsRequired();

        builder.HasIndex(whs => whs.CompletedAt);

        builder.Property(whs => whs.CreatedAt)
            .IsRequired();

        builder.Property(whs => whs.UpdatedAt);

        // Индексы
        builder.HasIndex(whs => new { whs.WorkoutHistoryId, whs.ExerciseId, whs.SetNumber });
        builder.HasIndex(whs => whs.ExerciseId);

        // Связи
        builder.HasOne(whs => whs.WorkoutHistory)
            .WithMany(wh => wh.Sets)
            .HasForeignKey(whs => whs.WorkoutHistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(whs => whs.Exercise)
            .WithMany()
            .HasForeignKey(whs => whs.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
