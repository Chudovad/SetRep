using Microsoft.AspNetCore.Mvc;
using SetRep.Core.Entities;
using SetRep.Core.Repositories;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Получить пользователя по Telegram ID
    /// </summary>
    [HttpGet("telegram/{telegramId}")]
    public async Task<ActionResult<User>> GetByTelegramId(Guid telegramId)
    {
        var user = await _userRepository.GetByTelegramIdAsync(telegramId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] User user)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }
        user.CreatedAt = DateTime.UtcNow;

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }
}

