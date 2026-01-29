using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetRep.API.Services;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;
using SetRep.Shared.DTO;
using static SetRep.Core.Constants;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeasurementsHistoryController : ControllerBase
{
    private readonly IMeasurementsHistoryRepository _measurementsHistoryRepository;
    private readonly ILogger<MeasurementsHistoryController> _logger;
    private readonly IYandexObjectStorageService _storageService;

    public MeasurementsHistoryController(
        IMeasurementsHistoryRepository measurementsHistoryRepository,
        ILogger<MeasurementsHistoryController> logger,
        IYandexObjectStorageService storageService)
    {
        _measurementsHistoryRepository = measurementsHistoryRepository;
        _logger = logger;
        _storageService = storageService;
    }

    /// <summary>
    /// Получить всю историю веса
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeasurementsHistory>>> GetAll()
    {
        var history = await _measurementsHistoryRepository.GetAllAsync();
        return Ok(history);
    }

    /// <summary>
    /// Получить запись истории веса по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementsHistory>> GetById(Guid id)
    {
        var record = await _measurementsHistoryRepository.GetByIdAsync(id);
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
    public async Task<ActionResult<IEnumerable<MeasurementsHistory>>> GetByUserId(Guid userId)
    {
        var history = await _measurementsHistoryRepository.GetByUserIdAsync(userId);
        return Ok(history);
    }

    /// <summary>
    /// Получить историю веса пользователя за период
    /// </summary>
    [HttpGet("user/{userId}/date-range")]
    public async Task<ActionResult<IEnumerable<MeasurementsHistory>>> GetByDateRange(
        Guid userId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var history = await _measurementsHistoryRepository.GetByUserIdAndDateRangeAsync(userId, startDate, endDate);
        return Ok(history);
    }

    /// <summary>
    /// Получить запись AllEntries — последние значения по каждому полю из всех замеров пользователя.
    /// </summary>
    [HttpGet("user/{userId}/last")]
    public async Task<ActionResult<MeasurementsHistory>> GetLast(Guid userId)
    {
        var record = await _measurementsHistoryRepository.GetByUserIdAndNotesAsync(userId, AllEntriesNotes);
        if (record == null)
        {
            return NotFound();
        }
        return Ok(record);
    }

    /// <summary>
    /// Создать новую запись измерений
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MeasurementsHistory>> Create([FromBody] CreateMeasurementsHistoryDto dto)
    {
        var measurementsHistory = new MeasurementsHistory
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Date = dto.Date,
            ProgressImageUrl = dto.ProgressImageUrl,
            BodyWeight = dto.BodyWeight,
            Waist = dto.Waist,
            BodyFatPercentage = dto.BodyFatPercentage,
            LeanBodyMass = dto.LeanBodyMass,
            Neck = dto.Neck,
            Shoulder = dto.Shoulder,
            Chest = dto.Chest,
            LeftBicep = dto.LeftBicep,
            RightBicep = dto.RightBicep,
            LeftForearm = dto.LeftForearm,
            RightForearm = dto.RightForearm,
            Buttocks = dto.Buttocks,
            LeftThigh = dto.LeftThigh,
            RightThigh = dto.RightThigh,
            LeftCalf = dto.LeftCalf,
            RightCalf = dto.RightCalf,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _measurementsHistoryRepository.AddAsync(measurementsHistory);
        await _measurementsHistoryRepository.SaveChangesAsync();

        await CreateOrUpdateAllEntriesAsync(dto.UserId, dto);

        return CreatedAtAction(nameof(GetById), new { id = measurementsHistory.Id }, measurementsHistory);
    }

    /// <summary>
    /// Создаёт или обновляет запись AllEntries — агрегат последних значений по каждому полю из всех замеров пользователя.
    /// </summary>
    private async Task CreateOrUpdateAllEntriesAsync(Guid userId, CreateMeasurementsHistoryDto dto)
    {
        var allEntries = await _measurementsHistoryRepository.GetByUserIdAndNotesAsync(userId, AllEntriesNotes);

        if (allEntries == null)
        {
            allEntries = new MeasurementsHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = dto.Date,
                ProgressImageUrl = dto.ProgressImageUrl,
                BodyWeight = dto.BodyWeight,
                Waist = dto.Waist,
                BodyFatPercentage = dto.BodyFatPercentage,
                LeanBodyMass = dto.LeanBodyMass,
                Neck = dto.Neck,
                Shoulder = dto.Shoulder,
                Chest = dto.Chest,
                LeftBicep = dto.LeftBicep,
                RightBicep = dto.RightBicep,
                LeftForearm = dto.LeftForearm,
                RightForearm = dto.RightForearm,
                Buttocks = dto.Buttocks,
                LeftThigh = dto.LeftThigh,
                RightThigh = dto.RightThigh,
                LeftCalf = dto.LeftCalf,
                RightCalf = dto.RightCalf,
                Notes = AllEntriesNotes,
                CreatedAt = DateTime.UtcNow
            };
            await _measurementsHistoryRepository.AddAsync(allEntries);
        }
        else
        {
            allEntries.Date = dto.Date;
            if (dto.ProgressImageUrl != null) allEntries.ProgressImageUrl = dto.ProgressImageUrl;
            if (dto.BodyWeight.HasValue) allEntries.BodyWeight = dto.BodyWeight;
            if (dto.Waist.HasValue) allEntries.Waist = dto.Waist;
            if (dto.BodyFatPercentage.HasValue) allEntries.BodyFatPercentage = dto.BodyFatPercentage;
            if (dto.LeanBodyMass.HasValue) allEntries.LeanBodyMass = dto.LeanBodyMass;
            if (dto.Neck.HasValue) allEntries.Neck = dto.Neck;
            if (dto.Shoulder.HasValue) allEntries.Shoulder = dto.Shoulder;
            if (dto.Chest.HasValue) allEntries.Chest = dto.Chest;
            if (dto.LeftBicep.HasValue) allEntries.LeftBicep = dto.LeftBicep;
            if (dto.RightBicep.HasValue) allEntries.RightBicep = dto.RightBicep;
            if (dto.LeftForearm.HasValue) allEntries.LeftForearm = dto.LeftForearm;
            if (dto.RightForearm.HasValue) allEntries.RightForearm = dto.RightForearm;
            if (dto.Buttocks.HasValue) allEntries.Buttocks = dto.Buttocks;
            if (dto.LeftThigh.HasValue) allEntries.LeftThigh = dto.LeftThigh;
            if (dto.RightThigh.HasValue) allEntries.RightThigh = dto.RightThigh;
            if (dto.LeftCalf.HasValue) allEntries.LeftCalf = dto.LeftCalf;
            if (dto.RightCalf.HasValue) allEntries.RightCalf = dto.RightCalf;
            allEntries.UpdatedAt = DateTime.UtcNow;
            await _measurementsHistoryRepository.UpdateAsync(allEntries);
        }

        await _measurementsHistoryRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Обновить запись измерений
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateMeasurementsHistoryDto dto)
    {
        var existing = await _measurementsHistoryRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        var oldImageUrl = existing.ProgressImageUrl;

        existing.Date = dto.Date;
        existing.ProgressImageUrl = dto.ProgressImageUrl;
        existing.BodyWeight = dto.BodyWeight;
        existing.Waist = dto.Waist;
        existing.BodyFatPercentage = dto.BodyFatPercentage;
        existing.LeanBodyMass = dto.LeanBodyMass;
        existing.Neck = dto.Neck;
        existing.Shoulder = dto.Shoulder;
        existing.Chest = dto.Chest;
        existing.LeftBicep = dto.LeftBicep;
        existing.RightBicep = dto.RightBicep;
        existing.LeftForearm = dto.LeftForearm;
        existing.RightForearm = dto.RightForearm;
        existing.Buttocks = dto.Buttocks;
        existing.LeftThigh = dto.LeftThigh;
        existing.RightThigh = dto.RightThigh;
        existing.LeftCalf = dto.LeftCalf;
        existing.RightCalf = dto.RightCalf;
        existing.Notes = dto.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await _measurementsHistoryRepository.UpdateAsync(existing);
        await _measurementsHistoryRepository.SaveChangesAsync();

        try
        {
            var newImageUrl = dto.ProgressImageUrl;
            var isImageRemoved = !string.IsNullOrWhiteSpace(oldImageUrl) &&
                                 string.IsNullOrWhiteSpace(newImageUrl);
            var isImageReplaced = !string.IsNullOrWhiteSpace(oldImageUrl) &&
                                  !string.IsNullOrWhiteSpace(newImageUrl) &&
                                  !string.Equals(oldImageUrl, newImageUrl, StringComparison.OrdinalIgnoreCase);

            if ((isImageRemoved || isImageReplaced) && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                await _storageService.DeleteFileAsync(oldImageUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при удалении старого изображения прогресса для MeasurementsHistory {MeasurementId}",
                id);
        }

        return NoContent();
    }

    /// <summary>
    /// Удалить запись истории веса
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var record = await _measurementsHistoryRepository.GetByIdAsync(id);
        if (record == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(record.ProgressImageUrl))
        {
            try
            {
                await _storageService.DeleteFileAsync(record.ProgressImageUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при удалении изображения прогресса {ImageUrl} для MeasurementsHistory {MeasurementId}",
                    record.ProgressImageUrl,
                    id);
            }
        }

        await _measurementsHistoryRepository.DeleteAsync(record);
        await _measurementsHistoryRepository.SaveChangesAsync();

        return NoContent();
    }
}

