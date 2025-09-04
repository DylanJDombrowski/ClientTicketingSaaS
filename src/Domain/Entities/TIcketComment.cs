using ClientTicketingSaaS.Domain.Common;

namespace ClientTicketingSaaS.Domain.Entities;

public class TicketComment : BaseAuditableEntity
{
    public int TenantId { get; set; }
    public int TicketId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    
    // Navigation properties - Domain only!
    public Tenant Tenant { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
}

