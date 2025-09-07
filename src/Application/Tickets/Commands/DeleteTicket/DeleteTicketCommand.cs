using ClientTicketingSaaS.Application.Common.Interfaces;

namespace ClientTicketingSaaS.Application.Tickets.Commands.DeleteTicket;

public record DeleteTicketCommand(int Id) : IRequest;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public DeleteTicketCommandHandler(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = await _context.Tickets
            .Where(t => t.Id == request.Id && t.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Tickets.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}