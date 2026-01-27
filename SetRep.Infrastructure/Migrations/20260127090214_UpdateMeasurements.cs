using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abdomen",
                table: "MeasurementsHistory");

            migrationBuilder.DropColumn(
                name: "Hips",
                table: "MeasurementsHistory");

            migrationBuilder.AddColumn<decimal>(
                name: "Buttocks",
                table: "MeasurementsHistory",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                comment: "Обхват ягодиц в сантиметрах");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Buttocks",
                table: "MeasurementsHistory");

            migrationBuilder.AddColumn<decimal>(
                name: "Abdomen",
                table: "MeasurementsHistory",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                comment: "Обхват живота в сантиметрах");

            migrationBuilder.AddColumn<decimal>(
                name: "Hips",
                table: "MeasurementsHistory",
                type: "numeric(6,2)",
                precision: 6,
                scale: 2,
                nullable: true,
                comment: "Обхват бедер в сантиметрах");
        }
    }
}
