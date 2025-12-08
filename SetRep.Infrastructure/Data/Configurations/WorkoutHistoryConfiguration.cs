using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности WorkoutHistory
/// </summary>
public class WorkoutHistoryConfiguration : IEntityTypeConfiguration<WorkoutHistory>
{
    public void Configure(EntityTypeBuilder<WorkoutHistory> builder)
    {
        builder.ToTable("WorkoutHistory");

        builder.HasKey(wh => wh.Id);

        builder.Property(wh => wh.Id)
            .IsRequired();

        builder.Property(wh => wh.WorkoutId)
            .IsRequired();

        builder.Property(wh => wh.UserId)
            .IsRequired();

        builder.Property(wh => wh.StartTime)
            .IsRequired();

        builder.HasIndex(wh => wh.StartTime);

        builder.Property(wh => wh.EndTime);

        builder.Property(wh => wh.Notes)
            .HasMaxLength(2000);

        builder.Property(wh => wh.CreatedAt)
            .IsRequired();

        builder.Property(wh => wh.UpdatedAt);

        // Индексы
        builder.HasIndex(wh => new { wh.UserId, wh.StartTime });
        builder.HasIndex(wh => wh.WorkoutId);

        // Связи
        builder.HasOne(wh => wh.Workout)
            .WithMany()
            .HasForeignKey(wh => wh.WorkoutId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wh => wh.User)
            .WithMany()
            .HasForeignKey(wh => wh.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(wh => wh.Sets)
            .WithOne(whs => whs.WorkoutHistory)
            .HasForeignKey(whs => whs.WorkoutHistoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
