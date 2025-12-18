using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutHistorySetsController : ControllerBase
{
    private readonly IWorkoutHistorySetRepository _workoutHistorySetRepository;
    private readonly ILogger<WorkoutHistorySetsController> _logger;

    public WorkoutHistorySetsController(IWorkoutHistorySetRepository workoutHistorySetRepository, ILogger<WorkoutHistorySetsController> logger)
    {
        _workoutHistorySetRepository = workoutHistorySetRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все подходы истории тренировок
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutHistorySet>>> GetAll()
    {
        var sets = await _workoutHistorySetRepository.GetAllAsync();
        return Ok(sets);
    }

    /// <summary>
    /// Получить подход истории тренировки по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutHistorySet>> GetById(Guid id)
    {
        var set = await _workoutHistorySetRepository.GetByIdAsync(id);
        if (set == null)
        {
            return NotFound();
        }
        return Ok(set);
    }

    /// <summary>
    /// Получить подходы истории тренировки
    /// </summary>
    [HttpGet("workout-history/{workoutHistoryId}")]
    public async Task<ActionResult<IEnumerable<WorkoutHistorySet>>> GetByWorkoutHistoryId(Guid workoutHistoryId)
    {
        var sets = await _workoutHistorySetRepository.GetByWorkoutHistoryIdAsync(workoutHistoryId);
        return Ok(sets);
    }

    /// <summary>
    /// Получить подходы упражнения в истории тренировки
    /// </summary>
    [HttpGet("workout-history/{workoutHistoryId}/exercise/{exerciseId}")]
    public async Task<ActionResult<IEnumerable<WorkoutHistorySet>>> GetByWorkoutHistoryIdAndExerciseId(Guid workoutHistoryId, Guid exerciseId)
    {
        var sets = await _workoutHistorySetRepository.GetByWorkoutHistoryIdAndExerciseIdAsync(workoutHistoryId, exerciseId);
        return Ok(sets);
    }

    /// <summary>
    /// Создать новый подход в истории тренировки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkoutHistorySet>> Create([FromBody] CreateWorkoutHistorySetDto dto)
    {
        var set = new WorkoutHistorySet
        {
            Id = Guid.NewGuid(),
            WorkoutHistoryId = dto.WorkoutHistoryId,
            ExerciseId = dto.ExerciseId,
            SetNumber = dto.SetNumber,
            Reps = dto.Reps,
            Weight = dto.Weight,
            DurationSeconds = dto.DurationSeconds,
            DistanceMeters = dto.DistanceMeters,
            RestSeconds = dto.RestSeconds,
            IsCompleted = dto.IsCompleted,
            Notes = dto.Notes,
            CompletedAt = dto.CompletedAt,
            CreatedAt = DateTime.UtcNow
        };

        await _workoutHistorySetRepository.AddAsync(set);
        await _workoutHistorySetRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = set.Id }, set);
    }

    /// <summary>
    /// Обновить подход в истории тренировки
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkoutHistorySetDto dto)
    {
        var existing = await _workoutHistorySetRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.WorkoutHistoryId = dto.WorkoutHistoryId;
        existing.ExerciseId = dto.ExerciseId;
        existing.SetNumber = dto.SetNumber;
        existing.Reps = dto.Reps;
        existing.Weight = dto.Weight;
        existing.DurationSeconds = dto.DurationSeconds;
        existing.DistanceMeters = dto.DistanceMeters;
        existing.RestSeconds = dto.RestSeconds;
        existing.IsCompleted = dto.IsCompleted;
        existing.Notes = dto.Notes;
        existing.CompletedAt = dto.CompletedAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await _workoutHistorySetRepository.UpdateAsync(existing);
        await _workoutHistorySetRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Обновить статус выполнения подхода
    /// </summary>
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> ToggleCompletion(Guid id, [FromBody] bool isCompleted)
    {
        var existing = await _workoutHistorySetRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.IsCompleted = isCompleted;
        if (isCompleted && existing.CompletedAt == default)
        {
            existing.CompletedAt = DateTime.UtcNow;
        }
        existing.UpdatedAt = DateTime.UtcNow;

        await _workoutHistorySetRepository.UpdateAsync(existing);
        await _workoutHistorySetRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить подход из истории тренировки
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var set = await _workoutHistorySetRepository.GetByIdAsync(id);
        if (set == null)
        {
            return NotFound();
        }

        await _workoutHistorySetRepository.DeleteAsync(set);
        await _workoutHistorySetRepository.SaveChangesAsync();

        return NoContent();
    }
}
