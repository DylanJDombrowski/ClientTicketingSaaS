using System.Security.Claims;

namespace ClientTicketingSaaS.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, int tenantId, string role);
    ClaimsPrincipal? ValidateToken(string token);
}