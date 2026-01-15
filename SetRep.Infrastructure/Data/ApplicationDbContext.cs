using Microsoft.EntityFrameworkCore;
using SetRep.Core.Entities;

namespace SetRep.Infrastructure.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Пользователи
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Упражнения
    /// </summary>
    public DbSet<Exercise> Exercises { get; set; } = null!;

    /// <summary>
    /// Тренировки
    /// </summary>
    public DbSet<Workout> Workouts { get; set; } = null!;

    /// <summary>
    /// Связь тренировок и упражнений
    /// </summary>
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; } = null!;

    /// <summary>
    /// Подходы
    /// </summary>
    public DbSet<Set> Sets { get; set; } = null!;

    /// <summary>
    /// История веса пользователей
    /// </summary>
    public DbSet<WeightHistory> WeightHistory { get; set; } = null!;

    /// <summary>
    /// История выполненных тренировок
    /// </summary>
    public DbSet<WorkoutHistory> WorkoutHistory { get; set; } = null!;

    /// <summary>
    /// Подходы в истории тренировок
    /// </summary>
    public DbSet<WorkoutHistorySet> WorkoutHistorySets { get; set; } = null!;

    /// <summary>
    /// Планы тренировок
    /// </summary>
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

