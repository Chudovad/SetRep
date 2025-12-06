using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutExercisesController : ControllerBase
{
    private readonly IWorkoutExerciseRepository _workoutExerciseRepository;
    private readonly ILogger<WorkoutExercisesController> _logger;

    public WorkoutExercisesController(
        IWorkoutExerciseRepository workoutExerciseRepository,
        ILogger<WorkoutExercisesController> logger)
    {
        _workoutExerciseRepository = workoutExerciseRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все связи тренировок и упражнений
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutExercise>>> GetAll()
    {
        var workoutExercises = await _workoutExerciseRepository.GetAllAsync();
        return Ok(workoutExercises);
    }

    /// <summary>
    /// Получить связь по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutExercise>> GetById(Guid id)
    {
        var workoutExercise = await _workoutExerciseRepository.GetByIdAsync(id);
        if (workoutExercise == null)
        {
            return NotFound();
        }
        return Ok(workoutExercise);
    }

    /// <summary>
    /// Получить связь с подходами
    /// </summary>
    [HttpGet("{id}/with-sets")]
    public async Task<ActionResult<WorkoutExercise>> GetWithSets(Guid id)
    {
        var workoutExercise = await _workoutExerciseRepository.GetWithSetsAsync(id);
        if (workoutExercise == null)
        {
            return NotFound();
        }
        return Ok(workoutExercise);
    }

    /// <summary>
    /// Получить упражнения тренировки
    /// </summary>
    [HttpGet("workout/{workoutId}")]
    public async Task<ActionResult<IEnumerable<WorkoutExercise>>> GetByWorkoutId(Guid workoutId)
    {
        var workoutExercises = await _workoutExerciseRepository.GetByWorkoutIdAsync(workoutId);
        return Ok(workoutExercises);
    }

    /// <summary>
    /// Создать новую связь тренировки и упражнения
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkoutExercise>> Create([FromBody] CreateWorkoutExerciseDto dto)
    {
        var workoutExercise = new WorkoutExercise
        {
            Id = Guid.NewGuid(),
            WorkoutId = dto.WorkoutId,
            ExerciseId = dto.ExerciseId,
            Order = dto.Order,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _workoutExerciseRepository.AddAsync(workoutExercise);
        await _workoutExerciseRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workoutExercise.Id }, workoutExercise);
    }

    /// <summary>
    /// Обновить связь тренировки и упражнения
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkoutExerciseDto dto)
    {
        var existing = await _workoutExerciseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.WorkoutId = dto.WorkoutId;
        existing.ExerciseId = dto.ExerciseId;
        existing.Order = dto.Order;
        existing.Notes = dto.Notes;

        await _workoutExerciseRepository.UpdateAsync(existing);
        await _workoutExerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить связь тренировки и упражнения
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workoutExercise = await _workoutExerciseRepository.GetByIdAsync(id);
        if (workoutExercise == null)
        {
            return NotFound();
        }

        await _workoutExerciseRepository.DeleteAsync(workoutExercise);
        await _workoutExerciseRepository.SaveChangesAsync();

        return NoContent();
    }
}

