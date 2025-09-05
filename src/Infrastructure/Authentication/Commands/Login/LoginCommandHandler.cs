using ClientTicketingSaaS.Application.Common.Interfaces;
namespace ClientTicketingSaaS.Infrastructure.Authentication.Commands.Login;
using Microsoft.AspNetCore.Identity;
using ClientTicketingSaaS.Infrastructure.Identity;
using ClientTicketingSaaS.Application.Auth.Commands.Login; // for LoginCommand
using MediatR;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IApplicationDbContext context,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return new LoginResult { IsSuccess = false, Error = "Invalid credentials" };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return new LoginResult { IsSuccess = false, Error = "Invalid credentials" };
        }

        // Get tenant info
        var tenant = await _context.Tenants.FindAsync(user.TenantId, cancellationToken);
        if (tenant == null || !tenant.IsActive)
        {
            return new LoginResult { IsSuccess = false, Error = "Account not accessible" };
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Email!, user.TenantId, "User");

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            TenantId = user.TenantId,
            TenantName = tenant.Name,
            Role = "User",
            IsActive = user.IsActive
        };

        return new LoginResult 
        { 
            IsSuccess = true, 
            Token = token,
            User = userDto 
        };
    }
}