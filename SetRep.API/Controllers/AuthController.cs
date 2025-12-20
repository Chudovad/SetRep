using Microsoft.AspNetCore.Mvc;
using SetRep.API.Services;
using SetRep.Core.Repositories;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITelegramWebAppValidationService _validationService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ITelegramWebAppValidationService validationService,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _validationService = validationService;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Аутентификация через Telegram Web App данные
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TgWebAppData))
        {
            return BadRequest(new { error = "tgWebAppData is required" });
        }

        var botToken = _configuration["BotToken"];
        if (string.IsNullOrWhiteSpace(botToken))
        {
            return StatusCode(500, new { error = "BotToken is not configured" });
        }

        // Валидируем Telegram Web App данные
        var validationResult = _validationService.Validate(request.TgWebAppData, botToken);

        if (!validationResult.IsValid || validationResult.Data == null)
        {
            return Unauthorized(new { error = validationResult.ErrorMessage ?? "Invalid Telegram Web App data" });
        }

        var telegramUser = validationResult.Data.User;
        
        // Находим или создаем пользователя в базе данных
        var user = await _userRepository.GetByTelegramIdAsync(telegramUser.TelegramId);
        
        if (user == null)
        {
            // Создаем нового пользователя
            user = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramUser.TelegramId,
                Name = $"{telegramUser.FirstName} {telegramUser.LastName}".Trim(),
                Username = telegramUser.Username ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        else
        {
            // Обновляем информацию пользователя, если нужно
            var fullName = $"{telegramUser.FirstName} {telegramUser.LastName}".Trim();
            if (user.Name != fullName || user.Username != (telegramUser.Username ?? string.Empty))
            {
                user.Name = fullName;
                user.Username = telegramUser.Username ?? string.Empty;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
            }
        }

        // Генерируем JWT токен
        var token = _jwtTokenService.GenerateToken(
            telegramUser.TelegramId,
            user.Id,
            user.Username ?? user.Name
        );

        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                telegramId = user.TelegramId,
                name = user.Name,
                username = user.Username
            }
        });
    }
}

public class LoginRequest
{
    public string TgWebAppData { get; set; } = string.Empty;
}

