using ClientTicketingSaaS.Domain.Common;

namespace ClientTicketingSaaS.Domain.Entities;

public class TimeEntry : BaseAuditableEntity
{
    public int TenantId { get; set; }
    public int TicketId { get; set; }
    public string UserId { get; set; } = string.Empty; // Just store the ID as string
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public bool IsBillable { get; set; } = true;
    
    // Navigation properties - DOMAIN ENTITIES ONLY!
    public Tenant Tenant { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
    // No ApplicationUser here - Domain doesn't know about it!
}