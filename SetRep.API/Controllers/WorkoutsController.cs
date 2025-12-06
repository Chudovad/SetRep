using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ILogger<WorkoutsController> _logger;

    public WorkoutsController(IWorkoutRepository workoutRepository, ILogger<WorkoutsController> logger)
    {
        _workoutRepository = workoutRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить все тренировки
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workout>>> GetAll()
    {
        var workouts = await _workoutRepository.GetAllAsync();
        return Ok(workouts);
    }

    /// <summary>
    /// Получить тренировку по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Workout>> GetById(Guid id)
    {
        var workout = await _workoutRepository.GetByIdAsync(id);
        if (workout == null)
        {
            return NotFound();
        }
        return Ok(workout);
    }

    /// <summary>
    /// Получить тренировку с деталями (упражнениями и подходами)
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<ActionResult<Workout>> GetWithDetails(Guid id)
    {
        var workout = await _workoutRepository.GetWithDetailsAsync(id);
        if (workout == null)
        {
            return NotFound();
        }
        return Ok(workout);
    }

    /// <summary>
    /// Получить тренировки пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Workout>>> GetByUserId(Guid userId)
    {
        var workouts = await _workoutRepository.GetByUserIdAsync(userId);
        return Ok(workouts);
    }

    /// <summary>
    /// Получить тренировки пользователя за период
    /// </summary>
    [HttpGet("user/{userId}/date-range")]
    public async Task<ActionResult<IEnumerable<Workout>>> GetByDateRange(
        Guid userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var workouts = await _workoutRepository.GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
        return Ok(workouts);
    }

    /// <summary>
    /// Получить последнюю тренировку пользователя
    /// </summary>
    [HttpGet("user/{userId}/last")]
    public async Task<ActionResult<Workout>> GetLastWorkout(Guid userId)
    {
        var workout = await _workoutRepository.GetLastWorkoutByUserIdAsync(userId);
        if (workout == null)
        {
            return NotFound();
        }
        return Ok(workout);
    }

    /// <summary>
    /// Создать новую тренировку
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Workout>> Create([FromBody] CreateWorkoutDto dto)
    {
        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Name = dto.Name,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _workoutRepository.AddAsync(workout);
        await _workoutRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }

    /// <summary>
    /// Обновить тренировку
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkoutDto dto)
    {
        var existingWorkout = await _workoutRepository.GetByIdAsync(id);
        if (existingWorkout == null)
        {
            return NotFound();
        }

        existingWorkout.Name = dto.Name;
        existingWorkout.UserId = dto.UserId;
        existingWorkout.StartTime = dto.StartTime;
        existingWorkout.EndTime = dto.EndTime;
        existingWorkout.Notes = dto.Notes;
        existingWorkout.UpdatedAt = DateTime.UtcNow;

        await _workoutRepository.UpdateAsync(existingWorkout);
        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить тренировку
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workout = await _workoutRepository.GetByIdAsync(id);
        if (workout == null)
        {
            return NotFound();
        }

        await _workoutRepository.DeleteAsync(workout);
        await _workoutRepository.SaveChangesAsync();

        return NoContent();
    }
}

