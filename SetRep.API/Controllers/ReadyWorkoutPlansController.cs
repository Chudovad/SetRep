using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.API.Extensions;
using SetRep.API.Options;
using SetRep.Core.Entities;
using SetRep.Core.Enums;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReadyWorkoutPlansController : ControllerBase
{
    private readonly IReadyWorkoutPlanRepository _repository;
    private readonly IWorkoutPlanRepository _workoutPlanRepository;
    private readonly ReadyWorkoutPlanOptions _options;
    private readonly ILogger<ReadyWorkoutPlansController> _logger;

    public ReadyWorkoutPlansController(
        IReadyWorkoutPlanRepository repository,
        IWorkoutPlanRepository workoutPlanRepository,
        Microsoft.Extensions.Options.IOptions<ReadyWorkoutPlanOptions> options,
        ILogger<ReadyWorkoutPlansController> logger)
    {
        _repository = repository;
        _workoutPlanRepository = workoutPlanRepository;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Получить список готовых планов с опциональными фильтрами
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadyWorkoutPlan>>> GetFiltered(
        [FromQuery] ReadyPlanLevel? level,
        [FromQuery] ReadyPlanGoal? goal,
        [FromQuery] ReadyPlanEquipment? equipment)
    {
        var list = await _repository.GetFilteredAsync(level, goal, equipment);
        return Ok(list);
    }

    /// <summary>
    /// Получить запись готового плана по идентификатору плана тренировки
    /// </summary>
    [HttpGet("by-plan/{planId:guid}")]
    public async Task<ActionResult<ReadyWorkoutPlan>> GetByPlanId(Guid planId)
    {
        var ready = await _repository.GetByWorkoutPlanIdAsync(planId);
        if (ready == null)
            return NotFound();
        return Ok(ready);
    }

    /// <summary>
    /// Добавить план в список готовых (только для разрешённых пользователей)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReadyWorkoutPlan>> Create([FromBody] CreateReadyWorkoutPlanDto dto)
    {
        var telegramId = User.GetTelegramId();
        if (telegramId == null || !_options.AllowedEditorTelegramIds.Contains(telegramId.Value))
        {
            return Forbid();
        }

        var plan = await _workoutPlanRepository.GetByIdAsync(dto.WorkoutPlanId);
        if (plan == null)
            return NotFound("План тренировки не найден.");

        var exists = await _repository.ExistsByWorkoutPlanIdAsync(dto.WorkoutPlanId);
        if (exists)
            return Conflict("Этот план уже добавлен в список готовых.");

        var entity = new ReadyWorkoutPlan
        {
            Id = Guid.NewGuid(),
            WorkoutPlanId = dto.WorkoutPlanId,
            Level = (ReadyPlanLevel)dto.Level,
            Goal = (ReadyPlanGoal)dto.Goal,
            Equipment = (ReadyPlanEquipment)dto.Equipment,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var withPlan = await _repository.GetByWorkoutPlanIdAsync(dto.WorkoutPlanId);
        return CreatedAtAction(nameof(GetByPlanId), new { planId = dto.WorkoutPlanId }, withPlan);
    }
}
