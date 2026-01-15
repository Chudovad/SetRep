using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkoutPlanId",
                table: "Workouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkoutPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutPlanId",
                table: "Workouts",
                column: "WorkoutPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlans_UserId",
                table: "WorkoutPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_WorkoutPlans_WorkoutPlanId",
                table: "Workouts",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_WorkoutPlans_WorkoutPlanId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "WorkoutPlans");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_WorkoutPlanId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanId",
                table: "Workouts");
        }
    }
}
