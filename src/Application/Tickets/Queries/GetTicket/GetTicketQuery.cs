using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Application.Tickets.Queries.GetTickets;

namespace ClientTicketingSaaS.Application.Tickets.Queries.GetTicket;

public record GetTicketQuery(int Id) : IRequest<TicketDto>;

public class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, TicketDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public GetTicketQueryHandler(IApplicationDbContext context, ITenantService tenantService, IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var ticket = await _context.Tickets
            .Where(t => t.Id == request.Id && t.TenantId == tenantId)
            .Include(t => t.Client)
            .Include(t => t.Comments.OrderBy(c => c.Created))
            .Include(t => t.TimeEntries.OrderByDescending(te => te.StartTime))
            .ProjectTo<TicketDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, ticket);

        return ticket;
    }
}