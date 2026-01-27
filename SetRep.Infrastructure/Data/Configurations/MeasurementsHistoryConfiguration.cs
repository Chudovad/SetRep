using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности MeasurementsHistory
/// </summary>
public class MeasurementsHistoryConfiguration : IEntityTypeConfiguration<MeasurementsHistory>
{
    public void Configure(EntityTypeBuilder<MeasurementsHistory> builder)
    {
        builder.ToTable("MeasurementsHistory");

        builder.HasKey(wh => wh.Id);

        builder.Property(wh => wh.Id)
            .IsRequired();

        builder.Property(wh => wh.UserId)
            .IsRequired();

        builder.Property(wh => wh.Date)
            .IsRequired();

        builder.HasIndex(wh => wh.Date);

        builder.Property(wh => wh.ProgressImageUrl)
            .HasMaxLength(500)
            .HasComment("URL изображения прогресса");

        builder.Property(wh => wh.BodyWeight)
            .HasPrecision(6, 2)
            .HasComment("Масса тела в килограммах");

        builder.Property(wh => wh.Waist)
            .HasPrecision(6, 2)
            .HasComment("Обхват талии в сантиметрах");

        builder.Property(wh => wh.BodyFatPercentage)
            .HasPrecision(5, 2)
            .HasComment("Содержание жира в процентах");

        builder.Property(wh => wh.LeanBodyMass)
            .HasPrecision(6, 2)
            .HasComment("Сухая мышечная масса в килограммах");

        builder.Property(wh => wh.Neck)
            .HasPrecision(6, 2)
            .HasComment("Обхват шеи в сантиметрах");

        builder.Property(wh => wh.Shoulder)
            .HasPrecision(6, 2)
            .HasComment("Обхват плеча в сантиметрах");

        builder.Property(wh => wh.Chest)
            .HasPrecision(6, 2)
            .HasComment("Обхват груди в сантиметрах");

        builder.Property(wh => wh.LeftBicep)
            .HasPrecision(6, 2)
            .HasComment("Обхват левого бицепса в сантиметрах");

        builder.Property(wh => wh.RightBicep)
            .HasPrecision(6, 2)
            .HasComment("Обхват правого бицепса в сантиметрах");

        builder.Property(wh => wh.LeftForearm)
            .HasPrecision(6, 2)
            .HasComment("Обхват левого предплечья в сантиметрах");

        builder.Property(wh => wh.RightForearm)
            .HasPrecision(6, 2)
            .HasComment("Обхват правого предплечья в сантиметрах");

        builder.Property(wh => wh.Buttocks)
            .HasPrecision(6, 2)
            .HasComment("Обхват ягодиц в сантиметрах");

        builder.Property(wh => wh.LeftThigh)
            .HasPrecision(6, 2)
            .HasComment("Обхват левого бедра в сантиметрах");

        builder.Property(wh => wh.RightThigh)
            .HasPrecision(6, 2)
            .HasComment("Обхват правого бедра в сантиметрах");

        builder.Property(wh => wh.LeftCalf)
            .HasPrecision(6, 2)
            .HasComment("Обхват левой икры в сантиметрах");

        builder.Property(wh => wh.RightCalf)
            .HasPrecision(6, 2)
            .HasComment("Обхват правой икры в сантиметрах");

        builder.Property(wh => wh.Notes)
            .HasMaxLength(1000);

        builder.Property(wh => wh.CreatedAt)
            .IsRequired();

        builder.Property(wh => wh.UpdatedAt);

        // Индексы
        builder.HasIndex(wh => new { wh.UserId, wh.Date });

        // Связи
        builder.HasOne(wh => wh.User)
            .WithMany(u => u.MeasurementsHistory)
            .HasForeignKey(wh => wh.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

