using ClientTicketingSaaS.Domain.Constants;
using ClientTicketingSaaS.Domain.Entities;
using ClientTicketingSaaS.Domain.Enums;
using ClientTicketingSaaS.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClientTicketingSaaS.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // Use migrations instead of EnsureCreated
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Create default tenant
        if (!_context.Tenants.Any())
        {
            var defaultTenant = new Tenant
            {
                Name = "Demo Company",
                Subdomain = "demo",
                IsActive = true,
                Plan = SubscriptionPlan.Professional,
                MaxUsers = 10,
                MaxTickets = 500,
                CurrentUsers = 0,
                CurrentTickets = 0
            };

            _context.Tenants.Add(defaultTenant);
            await _context.SaveChangesAsync(); // Save tenant first

            // Create default admin user for the tenant
            var administrator = new ApplicationUser 
            { 
                UserName = "admin@demo.com", 
                Email = "admin@demo.com",
                FirstName = "Admin",
                LastName = "User",
                TenantId = defaultTenant.Id,
                IsActive = true
            };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "Administrator1!");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
                }
            }

            // Create sample client
            var sampleClient = new Client
            {
                TenantId = defaultTenant.Id,
                Name = "Acme Corporation",
                Email = "contact@acme.com",
                Phone = "555-123-4567",
                Company = "Acme Corporation",
                IsActive = true
            };

            _context.Clients.Add(sampleClient);
            await _context.SaveChangesAsync(); // SAVE CLIENT FIRST to get the ID

            // NOW create ticket with the actual client ID
            var sampleTicket = new Ticket
            {
                TenantId = defaultTenant.Id,
                Title = "Welcome to your new ticketing system!",
                Description = "This is a sample ticket to demonstrate the system. You can edit or delete this ticket.",
                Status = TicketStatus.Open,
                Priority = TicketPriority.Medium,
                ClientId = sampleClient.Id, // Now this has a real value
                EstimatedHours = 2,
                ActualHours = 0
            };

            _context.Tickets.Add(sampleTicket);
            await _context.SaveChangesAsync(); // Save ticket
        }
    }
}