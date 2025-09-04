using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Clients.Commands.CreateClient;

public record CreateClientCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Company { get; init; }
}

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public CreateClientCommandHandler(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = new Client
        {
            TenantId = tenantId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Company = request.Company,
            IsActive = true
        };

        _context.Clients.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}