using ClientTicketingSaaS.Domain.Common;
using ClientTicketingSaaS.Domain.Enums;

namespace ClientTicketingSaaS.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string? CustomDomain { get; set; }
    public bool IsActive { get; set; } = true;
    public SubscriptionPlan Plan { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }
    
    // Usage tracking
    public int MaxUsers { get; set; } = 5;
    public int MaxTickets { get; set; } = 100;
    public int CurrentUsers { get; set; }
    public int CurrentTickets { get; set; }
    
    // Navigation properties - Domain doesn't know about ApplicationUser!
    // We'll set these up in the Infrastructure layer with EF configurations
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}