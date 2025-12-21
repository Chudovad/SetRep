using System.Security.Claims;

namespace SetRep.API.Services;

public interface IJwtTokenService
{
    string GenerateToken(long telegramId, Guid userId, string username);
    ClaimsPrincipal? ValidateToken(string token);
}

