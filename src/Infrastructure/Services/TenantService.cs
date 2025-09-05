using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ClientTicketingSaaS.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int _tenantId;
    
    public TenantService(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentTenantId()
    {
        // First, try to get from JWT token
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out var tokenTenantId))
            {
                return tokenTenantId;
            }
        }

        // Fallback to manually set tenant ID
        if (_tenantId != 0)
        {
            return _tenantId;
        }

        // Final fallback to default tenant for development
        var defaultTenant = _context.Tenants.FirstOrDefault();
        if (defaultTenant != null)
        {
            return defaultTenant.Id;
        }
        
        throw new InvalidOperationException("Tenant ID has not been set and no default tenant found. Multi-tenant context is required.");
    }

    public async Task<Tenant?> GetCurrentTenantAsync()
    {
        var tenantId = GetCurrentTenantId();
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId);
    }

    public async Task<bool> IsValidTenantAsync(string subdomain)
    {
        return await _context.Tenants
            .AnyAsync(t => t.Subdomain == subdomain && t.IsActive);
    }

    public void SetTenant(int tenantId)
    {
        _tenantId = tenantId;
    }

    public async Task<bool> IsWithinUsageLimitsAsync(int tenantId, string resourceType)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant == null) return false;

        return resourceType.ToLower() switch
        {
            "tickets" => tenant.CurrentTickets < tenant.MaxTickets,
            "users" => tenant.CurrentUsers < tenant.MaxUsers,
            _ => true
        };
    }
}