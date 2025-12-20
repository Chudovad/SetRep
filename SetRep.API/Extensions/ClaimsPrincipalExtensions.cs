using System.Security.Claims;

namespace SetRep.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получить ID пользователя из Claims
    /// </summary>
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }

    /// <summary>
    /// Получить Telegram ID пользователя из Claims
    /// </summary>
    public static long? GetTelegramId(this ClaimsPrincipal user)
    {
        var telegramIdClaim = user?.FindFirst("telegramId")?.Value;
        if (long.TryParse(telegramIdClaim, out var telegramId))
        {
            return telegramId;
        }
        return null;
    }

    /// <summary>
    /// Получить имя пользователя из Claims
    /// </summary>
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.Name)?.Value;
    }
}

