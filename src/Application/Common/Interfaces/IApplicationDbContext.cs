using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Client> Clients { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<TicketComment> TicketComments { get; }
    DbSet<TimeEntry> TimeEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}