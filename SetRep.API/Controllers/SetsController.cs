using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<Set>> Create([FromBody] Set set)
    {
        if (set.Id == Guid.Empty)
        {
            set.Id = Guid.NewGuid();
        }
        set.CreatedAt = DateTime.UtcNow;

        await _setRepository.AddAsync(set);
        await _setRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = set.Id }, set);
    }

    /// <summary>
    /// Обновить подход
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Set set)
    {
        if (id != set.Id)
        {
            return BadRequest();
        }

        var existing = await _setRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        set.UpdatedAt = DateTime.UtcNow;
        await _setRepository.UpdateAsync(set);
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

