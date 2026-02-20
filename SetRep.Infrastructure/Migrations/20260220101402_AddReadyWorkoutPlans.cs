using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReadyWorkoutPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadyWorkoutPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Goal = table.Column<int>(type: "integer", nullable: false),
                    Equipment = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadyWorkoutPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadyWorkoutPlans_WorkoutPlans_WorkoutPlanId",
                        column: x => x.WorkoutPlanId,
                        principalTable: "WorkoutPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadyWorkoutPlans_Equipment",
                table: "ReadyWorkoutPlans",
                column: "Equipment");

            migrationBuilder.CreateIndex(
                name: "IX_ReadyWorkoutPlans_Goal",
                table: "ReadyWorkoutPlans",
                column: "Goal");

            migrationBuilder.CreateIndex(
                name: "IX_ReadyWorkoutPlans_Level",
                table: "ReadyWorkoutPlans",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_ReadyWorkoutPlans_WorkoutPlanId",
                table: "ReadyWorkoutPlans",
                column: "WorkoutPlanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadyWorkoutPlans");
        }
    }
}
