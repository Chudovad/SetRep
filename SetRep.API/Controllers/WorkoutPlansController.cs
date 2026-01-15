using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ILogger<WorkoutPlansController> _logger;

    public WorkoutPlansController(
        IWorkoutPlanRepository workoutPlanRepository, 
        ILogger<WorkoutPlansController> logger)
    {
        _workoutPlanRepository = workoutPlanRepository;
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
}
