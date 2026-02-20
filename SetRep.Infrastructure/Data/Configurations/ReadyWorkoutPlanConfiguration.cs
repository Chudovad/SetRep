using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности ReadyWorkoutPlan
/// </summary>
public class ReadyWorkoutPlanConfiguration : IEntityTypeConfiguration<ReadyWorkoutPlan>
{
    public void Configure(EntityTypeBuilder<ReadyWorkoutPlan> builder)
    {
        builder.ToTable("ReadyWorkoutPlans");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired();

        builder.Property(r => r.WorkoutPlanId)
            .IsRequired();

        builder.Property(r => r.Level)
            .IsRequired();

        builder.Property(r => r.Goal)
            .IsRequired();

        builder.Property(r => r.Equipment)
            .IsRequired();

        builder.Property(r => r.Note)
            .HasMaxLength(2000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => r.Level);
        builder.HasIndex(r => r.Goal);
        builder.HasIndex(r => r.Equipment);
        builder.HasIndex(r => r.WorkoutPlanId)
            .IsUnique();

        builder.HasOne(r => r.WorkoutPlan)
            .WithMany()
            .HasForeignKey(r => r.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
