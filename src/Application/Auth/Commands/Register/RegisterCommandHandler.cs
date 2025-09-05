using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;
using ClientTicketingSaaS.Domain.Enums;

namespace ClientTicketingSaaS.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegisterResult { IsSuccess = false, Error = "Email already in use" };
        }

        // Create tenant first
        var tenant = new Tenant
        {
            Name = request.TenantName,
            Subdomain = GenerateSubdomain(request.TenantName),
            IsActive = true,
            Plan = SubscriptionPlan.Professional,
            MaxUsers = 10,
            MaxTickets = 500,
            CurrentUsers = 0,
            CurrentTickets = 0
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // Create user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            TenantId = tenant.Id,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new RegisterResult { IsSuccess = false, Error = errors };
        }

        // Update tenant user count
        tenant.CurrentUsers = 1;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Email!, tenant.Id, "User");

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            Role = "User",
            IsActive = user.IsActive
        };

        return new RegisterResult 
        { 
            IsSuccess = true, 
            Token = token,
            User = userDto 
        };
    }

    private static string GenerateSubdomain(string tenantName)
    {
        // Simple subdomain generation
        return tenantName.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("&", "and");
    }
}