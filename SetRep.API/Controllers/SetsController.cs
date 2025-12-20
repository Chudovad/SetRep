using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SetsController : ControllerBase
{
    private readonly ISetRepository _setRepository;
    private readonly ILogger<SetsController> _logger;

    public SetsController(ISetRepository setRepository, ILogger<SetsController> logger)
    {
        _setRepository = setRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все подходы
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Set>>> GetAll()
    {
        var sets = await _setRepository.GetAllAsync();
        return Ok(sets);
    }

    /// <summary>
    /// Получить подход по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Set>> GetById(Guid id)
    {
        var set = await _setRepository.GetByIdAsync(id);
        if (set == null)
        {
            return NotFound();
        }
        return Ok(set);
    }

    /// <summary>
    /// Получить подходы упражнения в тренировке
    /// </summary>
    [HttpGet("workout-exercise/{workoutExerciseId}")]
    public async Task<ActionResult<IEnumerable<Set>>> GetByWorkoutExerciseId(Guid workoutExerciseId)
    {
        var sets = await _setRepository.GetByWorkoutExerciseIdAsync(workoutExerciseId);
        return Ok(sets);
    }

    /// <summary>
    /// Получить подходы тренировки
    /// </summary>
    [HttpGet("workout/{workoutId}")]
    public async Task<ActionResult<IEnumerable<Set>>> GetByWorkoutId(Guid workoutId)
    {
        var sets = await _setRepository.GetByWorkoutIdAsync(workoutId);
        return Ok(sets);
    }

    /// <summary>
    /// Создать новый подход
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Set>> Create([FromBody] CreateSetDto dto)
    {
        var set = new Set
        {
            Id = Guid.NewGuid(),
            WorkoutExerciseId = dto.WorkoutExerciseId,
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

        await _setRepository.AddAsync(set);
        await _setRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = set.Id }, set);
    }

    /// <summary>
    /// Обновить подход
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateSetDto dto)
    {
        var existing = await _setRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.WorkoutExerciseId = dto.WorkoutExerciseId;
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

        await _setRepository.UpdateAsync(existing);
        await _setRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить подход
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var set = await _setRepository.GetByIdAsync(id);
        if (set == null)
        {
            return NotFound();
        }

        await _setRepository.DeleteAsync(set);
        await _setRepository.SaveChangesAsync();

        return NoContent();
    }
}

