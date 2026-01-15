using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности WorkoutPlan
/// </summary>
public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.ToTable("WorkoutPlans");

        builder.HasKey(wp => wp.Id);

        builder.Property(wp => wp.Id)
            .IsRequired();

        builder.Property(wp => wp.UserId)
            .IsRequired();

        builder.Property(wp => wp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(wp => wp.CreatedAt)
            .IsRequired();

        builder.Property(wp => wp.UpdatedAt);

        // Индексы
        builder.HasIndex(wp => wp.UserId);

        // Связи
        builder.HasMany(wp => wp.Workouts)
            .WithOne(w => w.WorkoutPlan)
            .HasForeignKey(w => w.WorkoutPlanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
