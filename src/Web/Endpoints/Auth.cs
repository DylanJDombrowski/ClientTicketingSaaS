using ClientTicketingSaaS.Application.Auth.Commands.Login;
using ClientTicketingSaaS.Application.Auth.Commands.Register;
using ClientTicketingSaaS.Application.Auth.Common; // Add this for the shared UserDto
using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Login, "login").AllowAnonymous();
        groupBuilder.MapPost(Register, "register").AllowAnonymous();
        groupBuilder.MapGet(GetCurrentUser, "me").RequireAuthorization();
        groupBuilder.MapPost(Logout, "logout").RequireAuthorization();
    }

    public async Task<IResult> Login(ISender sender, LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await sender.Send(command);
        
        if (result.IsSuccess)
        {
            return Results.Ok(new LoginResponse 
            { 
                Token = result.Token!, 
                User = result.User! // This is now Application.Auth.Common.UserDto
            });
        }
        
        return Results.BadRequest(new { message = result.Error });
    }

    public async Task<IResult> Register(ISender sender, RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.TenantName,
            request.FirstName,
            request.LastName
        );
        
        var result = await sender.Send(command);
        
        if (result.IsSuccess)
        {
            return Results.Created("/auth/me", new RegisterResponse 
            { 
                Token = result.Token!, 
                User = result.User! // This is now Application.Auth.Common.UserDto
            });
        }
        
        return Results.BadRequest(new { message = result.Error });
    }

    public async Task<IResult> GetCurrentUser(
        IUser currentUser, 
        ITenantService tenantService,
        UserManager<ApplicationUser> userManager)
    {
        if (currentUser.Id == null)
        {
            return Results.Unauthorized();
        }

        var user = await userManager.FindByIdAsync(currentUser.Id);
        if (user == null || !user.IsActive)
        {
            return Results.Unauthorized();
        }

        var tenant = await tenantService.GetCurrentTenantAsync();
        if (tenant == null || !tenant.IsActive)
        {
            return Results.Unauthorized();
        }

        
        var userDto = new Application.Auth.Common.UserDto // This now refers to the using statement above
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

        return Results.Ok(userDto);
    }

    public Task<IResult> Logout()
    {
        // For JWT-based auth, logout is handled client-side
        return Task.FromResult(Results.Ok(new { message = "Logged out successfully" }));
    }
}

// DTOs for the Web API - use the shared UserDto from Application layer
public record LoginRequest(string Email, string Password);

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public Application.Auth.Common.UserDto User { get; init; } = null!; // This refers to Application.Auth.Common.UserDto
}

public record RegisterRequest(
    string Email,
    string Password,
    string TenantName,
    string? FirstName = null,
    string? LastName = null
);

public record RegisterResponse
{
    public string Token { get; init; } = string.Empty;
    public Application.Auth.Common.UserDto User { get; init; } = null!; // This refers to Application.Auth.Common.UserDto
}

// Remove the local UserDto definition - we're using the shared one from Application.Auth.Common