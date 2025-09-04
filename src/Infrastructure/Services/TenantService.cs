using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientTicketingSaaS.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _context;
    private int _tenantId;
    
    public TenantService(IApplicationDbContext context)
    {
        _context = context;
    }

    public int GetCurrentTenantId()
    {
        if (_tenantId == 0)
        {
            throw new InvalidOperationException("Tenant ID has not been set. Multi-tenant context is required.");
        }
        return _tenantId;
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