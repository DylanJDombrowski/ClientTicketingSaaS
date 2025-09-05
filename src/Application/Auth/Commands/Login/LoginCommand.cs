namespace ClientTicketingSaaS.Application.Auth.Commands.Login;
using ClientTicketingSaaS.Application.Auth.Common; 

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult
{
    public bool IsSuccess { get; init; }
    public string? Token { get; init; }
    public UserDto? User { get; init; }
    public string? Error { get; init; }
}

