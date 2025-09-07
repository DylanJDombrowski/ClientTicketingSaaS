using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Application.Common.Mappings;

namespace ClientTicketingSaaS.Application.Tickets.Queries.GetTickets;

public record GetTicketsQuery : IRequest<IList<TicketDto>>
{
    public TicketStatus? Status { get; init; }
    public TicketPriority? Priority { get; init; }
    public int? ClientId { get; init; }
    public string? AssignedToId { get; init; }
}

public class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, IList<TicketDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public GetTicketsQueryHandler(IApplicationDbContext context, ITenantService tenantService, IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<IList<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var query = _context.Tickets
            .Where(t => t.TenantId == tenantId)
            .Include(t => t.Client)
            .AsQueryable();

        // Apply filters
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(t => t.Priority == request.Priority.Value);

        if (request.ClientId.HasValue)
            query = query.Where(t => t.ClientId == request.ClientId.Value);

        if (!string.IsNullOrEmpty(request.AssignedToId))
            query = query.Where(t => t.AssignedToId == request.AssignedToId);

        return await query
            .OrderByDescending(t => t.Created)
            .ProjectToListAsync<TicketDto>(_mapper.ConfigurationProvider, cancellationToken);
    }
}