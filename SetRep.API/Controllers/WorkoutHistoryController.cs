using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutHistoryController : ControllerBase
{
    private readonly IWorkoutHistoryRepository _workoutHistoryRepository;
    private readonly ILogger<WorkoutHistoryController> _logger;

    public WorkoutHistoryController(IWorkoutHistoryRepository workoutHistoryRepository, ILogger<WorkoutHistoryController> logger)
    {
        _workoutHistoryRepository = workoutHistoryRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить всю историю тренировок
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutHistory>>> GetAll()
    {
        var workoutHistory = await _workoutHistoryRepository.GetAllAsync();
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Получить запись истории тренировки по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutHistory>> GetById(Guid id)
    {
        var workoutHistory = await _workoutHistoryRepository.GetByIdAsync(id);
        if (workoutHistory == null)
        {
            return NotFound();
        }
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Получить запись истории тренировки с деталями (подходами)
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<ActionResult<WorkoutHistory>> GetWithDetails(Guid id)
    {
        var workoutHistory = await _workoutHistoryRepository.GetWithDetailsAsync(id);
        if (workoutHistory == null)
        {
            return NotFound();
        }
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Получить историю тренировок пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WorkoutHistory>>> GetByUserId(Guid userId)
    {
        var workoutHistory = await _workoutHistoryRepository.GetByUserIdAsync(userId);
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Получить последнюю тренировку пользователя из истории
    /// </summary>
    [HttpGet("user/{userId}/last")]
    public async Task<ActionResult<WorkoutHistory>> GetLastWorkoutHistory(Guid userId)
    {
        var workoutHistory = await _workoutHistoryRepository.GetLastWorkoutHistoryByUserIdAsync(userId);
        if (workoutHistory == null)
        {
            return NotFound();
        }
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Получить историю тренировок по шаблону тренировки
    /// </summary>
    [HttpGet("workout/{workoutId}")]
    public async Task<ActionResult<IEnumerable<WorkoutHistory>>> GetByWorkoutId(Guid workoutId)
    {
        var workoutHistory = await _workoutHistoryRepository.GetByWorkoutIdAsync(workoutId);
        return Ok(workoutHistory);
    }

    /// <summary>
    /// Создать новую запись истории тренировки
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WorkoutHistory>> Create([FromBody] CreateWorkoutHistoryDto dto)
    {
        var workoutHistory = new WorkoutHistory
        {
            Id = Guid.NewGuid(),
            WorkoutId = dto.WorkoutId,
            UserId = dto.UserId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _workoutHistoryRepository.AddAsync(workoutHistory);
        await _workoutHistoryRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = workoutHistory.Id }, workoutHistory);
    }

    /// <summary>
    /// Обновить запись истории тренировки
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateWorkoutHistoryDto dto)
    {
        var existing = await _workoutHistoryRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.WorkoutId = dto.WorkoutId;
        existing.UserId = dto.UserId;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;
        existing.Notes = dto.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await _workoutHistoryRepository.UpdateAsync(existing);
        await _workoutHistoryRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить запись истории тренировки
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var workoutHistory = await _workoutHistoryRepository.GetByIdAsync(id);
        if (workoutHistory == null)
        {
            return NotFound();
        }

        await _workoutHistoryRepository.DeleteAsync(workoutHistory);
        await _workoutHistoryRepository.SaveChangesAsync();

        return NoContent();
    }
}
