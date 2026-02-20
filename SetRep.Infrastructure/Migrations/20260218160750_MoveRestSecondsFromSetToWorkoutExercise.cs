using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveRestSecondsFromSetToWorkoutExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestSeconds",
                table: "Sets");

            migrationBuilder.AddColumn<int>(
                name: "RestSeconds",
                table: "WorkoutExercises",
                type: "integer",
                nullable: false,
                defaultValue: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestSeconds",
                table: "WorkoutExercises");

            migrationBuilder.AddColumn<int>(
                name: "RestSeconds",
                table: "Sets",
                type: "integer",
                nullable: true);
        }
    }
}
