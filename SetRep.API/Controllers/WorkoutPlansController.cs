using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.API.Extensions;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutPlansController : ControllerBase
{
    private readonly IWorkoutPlanRepository _workoutPlanRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IWorkoutExerciseRepository _workoutExerciseRepository;
    private readonly ISetRepository _setRepository;
    private readonly ILogger<WorkoutPlansController> _logger;

    public WorkoutPlansController(
        IWorkoutPlanRepository workoutPlanRepository,
        IWorkoutRepository workoutRepository,
        IWorkoutExerciseRepository workoutExerciseRepository,
        ISetRepository setRepository,
        ILogger<WorkoutPlansController> logger)
    {
        _workoutPlanRepository = workoutPlanRepository;
        _workoutRepository = workoutRepository;
        _workoutExerciseRepository = workoutExerciseRepository;
        _setRepository = setRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все планы тренировок для конкретного пользователя
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetAll([FromQuery] Guid userId)
    {
        var workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId);
        return Ok(workoutPlans);
    }

    /// <summary>
    /// Получить план тренировки по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutPlan>> GetById(Guid id)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(id);
        if (workoutPlan == null)
        {
            return NotFound();
        }
        return Ok(workoutPlan);
    }

    /// <summary>
    /// Получить план тренировки с деталями (тренировками)
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<ActionResult<WorkoutPlan>> GetWithDetails(Guid id)
    {
        var workoutPlan = await _workoutPlanRepository.GetWithWorkoutsAsync(id);
        if (workoutPlan == null)
        {
            return NotFound();
        }
        return Ok(workoutPlan);
    }

    /// <summary>
    /// Получить планы тренировок пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WorkoutPlan>>> GetByUserId(Guid userId)
    {
        var workoutPlans = await _workoutPlanRepository.GetByUserIdAsync(userId);
        return Ok(workoutPlans);
    }

    /// <summary>
    /// Создать новый план тренировки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkoutPlan>> Create([FromBody] CreateWorkoutPlanDto dto)
    {
        var workoutPlan = new WorkoutPlan
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _workoutPlanRepository.AddAsync(workoutPlan);
        await _workoutPlanRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workoutPlan.Id }, workoutPlan);
    }

    /// <summary>
    /// Обновить план тренировки
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkoutPlanDto dto)
    {
        var existingWorkoutPlan = await _workoutPlanRepository.GetByIdAsync(id);
        if (existingWorkoutPlan == null)
        {
            return NotFound();
        }

        existingWorkoutPlan.Name = dto.Name;
        existingWorkoutPlan.UserId = dto.UserId;
        existingWorkoutPlan.UpdatedAt = DateTime.UtcNow;

        await _workoutPlanRepository.UpdateAsync(existingWorkoutPlan);
        await _workoutPlanRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить план тренировки
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(id);
        if (workoutPlan == null)
        {
            return NotFound();
        }

        await _workoutPlanRepository.DeleteAsync(workoutPlan);
        await _workoutPlanRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Скопировать план тренировки к текущему пользователю (план, тренировки, упражнения, подходы)
    /// </summary>
    [HttpPost("{id}/copy")]
    public async Task<ActionResult<WorkoutPlan>> Copy(Guid id)
    {
        var targetUserId = User.GetUserId();
        if (targetUserId == null)
            return Unauthorized();

        var sourcePlan = await _workoutPlanRepository.GetWithWorkoutsAsync(id);
        if (sourcePlan == null)
            return NotFound();

        var newPlan = new WorkoutPlan
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId.Value,
            Name = sourcePlan.Name,
            CreatedAt = DateTime.UtcNow
        };
        await _workoutPlanRepository.AddAsync(newPlan);

        if (sourcePlan.Workouts != null)
        {
            foreach (var srcW in sourcePlan.Workouts.OrderBy(w => w.CreatedAt))
            {
                var exercisesWithSets = await _workoutExerciseRepository.GetByWorkoutIdAsync(srcW.Id);
                var newWorkout = new Workout
                {
                    Id = Guid.NewGuid(),
                    UserId = targetUserId.Value,
                    WorkoutPlanId = newPlan.Id,
                    Name = srcW.Name,
                    Notes = srcW.Notes,
                    CreatedAt = DateTime.UtcNow
                };
                await _workoutRepository.AddAsync(newWorkout);

                foreach (var srcWe in exercisesWithSets.OrderBy(we => we.Order))
                {
                    var newWe = new WorkoutExercise
                    {
                        Id = Guid.NewGuid(),
                        WorkoutId = newWorkout.Id,
                        ExerciseId = srcWe.ExerciseId,
                        Order = srcWe.Order,
                        Notes = srcWe.Notes,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _workoutExerciseRepository.AddAsync(newWe);

                    if (srcWe.Sets != null)
                    {
                        foreach (var srcSet in srcWe.Sets.OrderBy(s => s.SetNumber))
                        {
                            var newSet = new Set
                            {
                                Id = Guid.NewGuid(),
                                WorkoutExerciseId = newWe.Id,
                                SetNumber = srcSet.SetNumber,
                                Reps = srcSet.Reps,
                                Weight = srcSet.Weight,
                                DurationSeconds = srcSet.DurationSeconds,
                                DistanceMeters = srcSet.DistanceMeters,
                                RestSeconds = srcSet.RestSeconds,
                                IsCompleted = false,
                                Notes = srcSet.Notes,
                                CompletedAt = default,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _setRepository.AddAsync(newSet);
                        }
                    }
                }
            }
        }

        await _workoutPlanRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = newPlan.Id }, newPlan);
    }
}
