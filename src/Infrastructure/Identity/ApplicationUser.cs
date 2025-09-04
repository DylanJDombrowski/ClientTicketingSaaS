using Microsoft.AspNetCore.Identity;
using ClientTicketingSaaS.Domain.Entities; // Add this using statement!

namespace ClientTicketingSaaS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties - Infrastructure can reference Domain!
    public Tenant Tenant { get; set; } = null!;
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    // Note: We'll configure the Ticket -> ApplicationUser relationship in EF configuration
}