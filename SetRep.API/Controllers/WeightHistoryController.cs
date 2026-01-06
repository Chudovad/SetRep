using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeightHistoryController : ControllerBase
{
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly ILogger<WeightHistoryController> _logger;

    public WeightHistoryController(
        IWeightHistoryRepository weightHistoryRepository,
        ILogger<WeightHistoryController> logger)
    {
        _weightHistoryRepository = weightHistoryRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить всю историю веса
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeightHistory>>> GetAll()
    {
        var history = await _weightHistoryRepository.GetAllAsync();
        return Ok(history);
    }

    /// <summary>
    /// Получить запись истории веса по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WeightHistory>> GetById(Guid id)
    {
        var record = await _weightHistoryRepository.GetByIdAsync(id);
        if (record == null)
        {
            return NotFound();
        }
        return Ok(record);
    }

    /// <summary>
    /// Получить историю веса пользователя
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WeightHistory>>> GetByUserId(Guid userId)
    {
        var history = await _weightHistoryRepository.GetByUserIdAsync(userId);
        return Ok(history);
    }

    /// <summary>
    /// Получить историю веса пользователя за период
    /// </summary>
    [HttpGet("user/{userId}/date-range")]
    public async Task<ActionResult<IEnumerable<WeightHistory>>> GetByDateRange(
        Guid userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var history = await _weightHistoryRepository.GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
        return Ok(history);
    }

    /// <summary>
    /// Получить последнюю запись веса пользователя
    /// </summary>
    [HttpGet("user/{userId}/last")]
    public async Task<ActionResult<WeightHistory>> GetLast(Guid userId)
    {
        var record = await _weightHistoryRepository.GetLastByUserIdAsync(userId);
        if (record == null)
        {
            return NotFound();
        }
        return Ok(record);
    }

    /// <summary>
    /// Создать новую запись истории веса
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WeightHistory>> Create([FromBody] WeightHistory weightHistory)
    {
        if (weightHistory.Id == Guid.Empty)
        {
            weightHistory.Id = Guid.NewGuid();
        }
        weightHistory.CreatedAt = DateTime.UtcNow;

        await _weightHistoryRepository.AddAsync(weightHistory);
        await _weightHistoryRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = weightHistory.Id }, weightHistory);
    }

    /// <summary>
    /// Обновить запись истории веса
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] WeightHistory weightHistory)
    {
        if (id != weightHistory.Id)
        {
            return BadRequest();
        }

        var existing = await _weightHistoryRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        weightHistory.UpdatedAt = DateTime.UtcNow;
        await _weightHistoryRepository.UpdateAsync(weightHistory);
        await _weightHistoryRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить запись истории веса
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var record = await _weightHistoryRepository.GetByIdAsync(id);
        if (record == null)
        {
            return NotFound();
        }

        await _weightHistoryRepository.DeleteAsync(record);
        await _weightHistoryRepository.SaveChangesAsync();

        return NoContent();
    }
}

