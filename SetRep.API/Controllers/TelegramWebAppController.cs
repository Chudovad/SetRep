using Microsoft.AspNetCore.Mvc;
using SetRep.API.Services;

namespace SetRep.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelegramWebAppController : ControllerBase
{
    private readonly ITelegramWebAppValidationService _validationService;
    private readonly IConfiguration _configuration;

    public TelegramWebAppController(
        ITelegramWebAppValidationService validationService,
        IConfiguration configuration)
    {
        _validationService = validationService;
        _configuration = configuration;
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] ValidateTelegramWebAppRequest request)
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

        var result = _validationService.Validate(request.TgWebAppData, botToken);

        if (!result.IsValid)
        {
            return Unauthorized(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            isValid = true,
            data = result.Data
        });
    }
}

public class ValidateTelegramWebAppRequest
{
    public string TgWebAppData { get; set; } = string.Empty;
}

