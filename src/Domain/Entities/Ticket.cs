using ClientTicketingSaaS.Domain.Common;
using ClientTicketingSaaS.Domain.Enums;

namespace ClientTicketingSaaS.Domain.Entities;

public class Ticket : BaseAuditableEntity
{
    public int TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    
    public int ClientId { get; set; }
    public string? AssignedToId { get; set; } // Just store the UserId as string
    public DateTime? DueDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    
    // Navigation properties - Domain only knows about Domain entities!
    public Tenant Tenant { get; set; } = null!;
    public Client Client { get; set; } = null!;
    // Note: No ApplicationUser navigation property here - we'll handle that in Infrastructure
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}