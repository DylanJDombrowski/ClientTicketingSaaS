namespace ClientTicketingSaaS.Application.Auth.Commands.Register;

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

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public int TenantId { get; init; }
    public string TenantName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}