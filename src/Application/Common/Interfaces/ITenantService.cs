using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Common.Interfaces;

public interface ITenantService
{
    int GetCurrentTenantId();
    Task<Tenant?> GetCurrentTenantAsync();
    Task<bool> IsValidTenantAsync(string subdomain);
    void SetTenant(int tenantId);
    Task<bool> IsWithinUsageLimitsAsync(int tenantId, string resourceType);
}