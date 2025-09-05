using ClientTicketingSaaS.Application.Common.Interfaces;

namespace ClientTicketingSaaS.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(int Id) : IRequest;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public DeleteClientCommandHandler(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = await _context.Clients
            .Where(c => c.Id == request.Id && c.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // Check if client has tickets
        var hasTickets = await _context.Tickets
            .AnyAsync(t => t.ClientId == request.Id, cancellationToken);

        if (hasTickets)
        {
            throw new InvalidOperationException("Cannot delete client that has tickets. Archive the client instead.");
        }

        _context.Clients.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}