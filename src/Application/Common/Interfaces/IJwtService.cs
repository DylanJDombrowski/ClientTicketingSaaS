using System.Security.Claims;
using ClientTicketingSaaS.Infrastructure.Identity;

namespace ClientTicketingSaaS.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, int tenantId);
    ClaimsPrincipal? ValidateToken(string token);
}