using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetRep.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workouts_StartTime",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserId_StartTime",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Workouts");

            migrationBuilder.CreateTable(
                name: "WorkoutHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutHistory_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutHistorySets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Reps = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Weight = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    DistanceMeters = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    RestSeconds = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutHistorySets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutHistorySets_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkoutHistorySets_WorkoutHistory_WorkoutHistoryId",
                        column: x => x.WorkoutHistoryId,
                        principalTable: "WorkoutHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistory_StartTime",
                table: "WorkoutHistory",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistory_UserId_StartTime",
                table: "WorkoutHistory",
                columns: new[] { "UserId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistory_WorkoutId",
                table: "WorkoutHistory",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistorySets_CompletedAt",
                table: "WorkoutHistorySets",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistorySets_ExerciseId",
                table: "WorkoutHistorySets",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutHistorySets_WorkoutHistoryId_ExerciseId_SetNumber",
                table: "WorkoutHistorySets",
                columns: new[] { "WorkoutHistoryId", "ExerciseId", "SetNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutHistorySets");

            migrationBuilder.DropTable(
                name: "WorkoutHistory");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Workouts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Workouts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_StartTime",
                table: "Workouts",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId_StartTime",
                table: "Workouts",
                columns: new[] { "UserId", "StartTime" });
        }
    }
}
