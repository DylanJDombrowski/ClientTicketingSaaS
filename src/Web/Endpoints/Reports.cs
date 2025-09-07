using ClientTicketingSaaS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Reports : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetDashboardStats).RequireAuthorization();
        groupBuilder.MapGet(GetTimeReport, "time").RequireAuthorization();
        groupBuilder.MapGet(GetClientReport, "clients").RequireAuthorization();
    }

    public async Task<IResult> GetDashboardStats(ISender sender, ITenantService tenantService, IApplicationDbContext context)
    {
        var tenantId = tenantService.GetCurrentTenantId();

        // Get basic stats
        var totalClients = await context.Clients.CountAsync(c => c.TenantId == tenantId && c.IsActive);
        var totalTickets = await context.Tickets.CountAsync(t => t.TenantId == tenantId);
        var openTickets = await context.Tickets.CountAsync(t => t.TenantId == tenantId && t.Status == TicketStatus.Open);
        var inProgressTickets = await context.Tickets.CountAsync(t => t.TenantId == tenantId && t.Status == TicketStatus.InProgress);
        
        // Get overdue tickets
        var overdueTickets = await context.Tickets
            .CountAsync(t => t.TenantId == tenantId && 
                        t.DueDate.HasValue && 
                        t.DueDate < DateTime.Now && 
                        t.Status != TicketStatus.Closed);

        // Get total hours this month
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var totalHoursThisMonth = await context.TimeEntries
            .Where(te => te.TenantId == tenantId && te.StartTime >= startOfMonth)
            .SumAsync(te => te.Hours);

        // Get billable hours this month
        var billableHoursThisMonth = await context.TimeEntries
            .Where(te => te.TenantId == tenantId && te.StartTime >= startOfMonth && te.IsBillable)
            .SumAsync(te => te.Hours);

        var stats = new
        {
            TotalClients = totalClients,
            TotalTickets = totalTickets,
            OpenTickets = openTickets,
            InProgressTickets = inProgressTickets,
            OverdueTickets = overdueTickets,
            TotalHoursThisMonth = totalHoursThisMonth,
            BillableHoursThisMonth = billableHoursThisMonth
        };

        return Results.Ok(stats);
    }

    public async Task<IResult> GetTimeReport(
        ISender sender, 
        ITenantService tenantService, 
        IApplicationDbContext context,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? clientId = null)
    {
        var tenantId = tenantService.GetCurrentTenantId();
        
        startDate ??= DateTime.Now.AddDays(-30);
        endDate ??= DateTime.Now;

        var query = context.TimeEntries
            .Where(te => te.TenantId == tenantId && 
                        te.StartTime >= startDate && 
                        te.StartTime <= endDate)
            .Include(te => te.Ticket)
            .ThenInclude(t => t.Client);

        if (clientId.HasValue)
        {
            query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Domain.Entities.TimeEntry, Domain.Entities.Client>)query.Where(te => te.Ticket.ClientId == clientId.Value);
        }

        var timeEntries = await query
            .GroupBy(te => te.Ticket.Client.Name)
            .Select(g => new
            {
                ClientName = g.Key,
                TotalHours = g.Sum(te => te.Hours),
                BillableHours = g.Where(te => te.IsBillable).Sum(te => te.Hours),
                TicketCount = g.Select(te => te.TicketId).Distinct().Count()
            })
            .ToListAsync();

        return Results.Ok(timeEntries);
    }

    public async Task<IResult> GetClientReport(ISender sender, ITenantService tenantService, IApplicationDbContext context)
    {
        var tenantId = tenantService.GetCurrentTenantId();

        var clientStats = await context.Clients
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Company,
                c.Email,
                TotalTickets = c.Tickets.Count(),
                OpenTickets = c.Tickets.Count(t => t.Status == TicketStatus.Open),
                TotalHours = c.Tickets.SelectMany(t => t.TimeEntries).Sum(te => te.Hours),
                BillableHours = c.Tickets.SelectMany(t => t.TimeEntries).Where(te => te.IsBillable).Sum(te => te.Hours)
            })
            .ToListAsync();

        return Results.Ok(clientStats);
    }
}