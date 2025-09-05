namespace ClientTicketingSaaS.Application.Auth.Commands.Register;
using ClientTicketingSaaS.Application.Auth.Common; 

public record RegisterCommand(
    string Email,
    string Password,
    string TenantName,
    string? FirstName = null,
    string? LastName = null
) : IRequest<RegisterResult>;

public record RegisterResult
{
    public bool IsSuccess { get; init; }
    public string? Token { get; init; }
    public UserDto? User { get; init; }
    public string? Error { get; init; }
}

