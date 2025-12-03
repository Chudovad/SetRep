using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности WeightHistory
/// </summary>
public class WeightHistoryConfiguration : IEntityTypeConfiguration<WeightHistory>
{
    public void Configure(EntityTypeBuilder<WeightHistory> builder)
    {
        builder.ToTable("WeightHistory");

        builder.HasKey(wh => wh.Id);

        builder.Property(wh => wh.Id)
            .IsRequired();

        builder.Property(wh => wh.UserId)
            .IsRequired();

        builder.Property(wh => wh.Weight)
            .IsRequired()
            .HasPrecision(6, 2)
            .HasComment("Вес в килограммах");

        builder.Property(wh => wh.Date)
            .IsRequired();

        builder.HasIndex(wh => wh.Date);

        builder.Property(wh => wh.Notes)
            .HasMaxLength(1000);

        builder.Property(wh => wh.CreatedAt)
            .IsRequired();

        builder.Property(wh => wh.UpdatedAt);

        // Индексы
        builder.HasIndex(wh => new { wh.UserId, wh.Date });

        // Связи
        builder.HasOne(wh => wh.User)
            .WithMany(u => u.WeightHistory)
            .HasForeignKey(wh => wh.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

