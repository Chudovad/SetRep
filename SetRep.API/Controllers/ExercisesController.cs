using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Core.Repositories;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IWorkoutExerciseRepository _workoutExerciseRepository;
    private readonly ILogger<ExercisesController> _logger;

    public ExercisesController(
        IExerciseRepository exerciseRepository,
        IWorkoutExerciseRepository workoutExerciseRepository,
        ILogger<ExercisesController> logger)
    {
        _exerciseRepository = exerciseRepository;
        _workoutExerciseRepository = workoutExerciseRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все упражнения
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetAll()
    {
        var exercises = await _exerciseRepository.GetAllAsync();
        return Ok(exercises);
    }

    /// <summary>
    /// Получить упражнение по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetById(Guid id)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(id);
        if (exercise == null)
        {
            return NotFound();
        }
        return Ok(exercise);
    }

    /// <summary>
    /// Получить системные упражнения
    /// </summary>
    [HttpGet("system")]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetSystemExercises()
    {
        var exercises = await _exerciseRepository.GetSystemExercisesAsync();
        return Ok(exercises);
    }

    /// <summary>
    /// Получить упражнения пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetUserExercises(Guid userId)
    {
        var exercises = await _exerciseRepository.GetUserExercisesAsync(userId);
        return Ok(exercises);
    }

    /// <summary>
    /// Получить упражнения по группе мышц
    /// </summary>
    [HttpGet("muscle-group/{muscleGroup}")]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetByMuscleGroup(MuscleGroup muscleGroup)
    {
        var exercises = await _exerciseRepository.GetByMuscleGroupAsync(muscleGroup);
        return Ok(exercises);
    }

    /// <summary>
    /// Поиск упражнений по названию
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Exercise>>> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term is required");
        }

        var exercises = await _exerciseRepository.SearchByNameAsync(term);
        return Ok(exercises);
    }

    /// <summary>
    /// Создать новое упражнение
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Exercise>> Create([FromBody] Exercise exercise)
    {
        if (exercise.Id == Guid.Empty)
        {
            exercise.Id = Guid.NewGuid();
        }
        exercise.CreatedAt = DateTime.UtcNow;

        await _exerciseRepository.AddAsync(exercise);
        await _exerciseRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = exercise.Id }, exercise);
    }

    /// <summary>
    /// Обновить упражнение
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] Exercise exercise)
    {
        if (id != exercise.Id)
        {
            return BadRequest();
        }

        var existingExercise = await _exerciseRepository.GetByIdAsync(id);
        if (existingExercise == null)
        {
            return NotFound();
        }

        existingExercise.Name = exercise.Name;
        existingExercise.Image = exercise.Image ?? string.Empty;
        existingExercise.MainImage = exercise.MainImage ?? string.Empty;
        existingExercise.Description = exercise.Description;
        existingExercise.MuscleGroup = exercise.MuscleGroup;
        existingExercise.WeightUnit = exercise.WeightUnit;
        existingExercise.UpdatedAt = DateTime.UtcNow;

        await _exerciseRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить упражнение
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(id);
        if (exercise == null)
        {
            return NotFound();
        }

        var workoutExercises = await _workoutExerciseRepository.GetByExerciseIdAsync(id);
        foreach (var workoutExercise in workoutExercises)
        {
            await _workoutExerciseRepository.DeleteAsync(workoutExercise);
        }

        await _exerciseRepository.DeleteAsync(exercise);

        await _exerciseRepository.SaveChangesAsync();

        return NoContent();
    }
}

