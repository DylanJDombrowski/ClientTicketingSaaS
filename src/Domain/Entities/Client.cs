using ClientTicketingSaaS.Domain.Common;

namespace ClientTicketingSaaS.Domain.Entities;

public class Client : BaseAuditableEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}