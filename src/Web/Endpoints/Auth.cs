using ClientTicketingSaaS.Application.Auth.Commands.Login;
using ClientTicketingSaaS.Application.Auth.Commands.Register;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Login, "login").AllowAnonymous();
        groupBuilder.MapPost(Register, "register").AllowAnonymous();
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
                User = result.User! 
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
                User = result.User! 
            });
        }
        
        return Results.BadRequest(new { message = result.Error });
    }
}

// DTOs for the API
public record LoginRequest(string Email, string Password);

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public Application.Auth.Commands.Login.UserDto User { get; init; } = null!;
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
    public Application.Auth.Commands.Register.UserDto User { get; init; } = null!;
}