using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandWeightHistoryToMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeightHistory");

            migrationBuilder.CreateTable(
                name: "MeasurementsHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProgressImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "URL изображения прогресса"),
                    BodyWeight = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Масса тела в килограммах"),
                    Waist = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват талии в сантиметрах"),
                    BodyFatPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true, comment: "Содержание жира в процентах"),
                    LeanBodyMass = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Сухая мышечная масса в килограммах"),
                    Neck = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват шеи в сантиметрах"),
                    Shoulder = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват плеча в сантиметрах"),
                    Chest = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват груди в сантиметрах"),
                    LeftBicep = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват левого бицепса в сантиметрах"),
                    RightBicep = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват правого бицепса в сантиметрах"),
                    LeftForearm = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват левого предплечья в сантиметрах"),
                    RightForearm = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват правого предплечья в сантиметрах"),
                    Abdomen = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват живота в сантиметрах"),
                    Hips = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват бедер в сантиметрах"),
                    LeftThigh = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват левого бедра в сантиметрах"),
                    RightThigh = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват правого бедра в сантиметрах"),
                    LeftCalf = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват левой икры в сантиметрах"),
                    RightCalf = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: true, comment: "Обхват правой икры в сантиметрах"),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementsHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementsHistory_Date",
                table: "MeasurementsHistory",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementsHistory_UserId_Date",
                table: "MeasurementsHistory",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementsHistory");

            migrationBuilder.CreateTable(
                name: "WeightHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false, comment: "Вес в килограммах")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeightHistory_Date",
                table: "WeightHistory",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WeightHistory_UserId_Date",
                table: "WeightHistory",
                columns: new[] { "UserId", "Date" });
        }
    }
}
