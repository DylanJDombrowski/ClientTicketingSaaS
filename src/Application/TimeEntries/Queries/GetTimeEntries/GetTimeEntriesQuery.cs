using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Application.Common.Mappings;
using ClientTicketingSaaS.Application.Common.DTOs;

namespace ClientTicketingSaaS.Application.TimeEntries.Queries.GetTimeEntries;

public record GetTimeEntriesQuery : IRequest<IList<EnhancedTimeEntryDto>>
{
    public int? TicketId { get; init; }
    public string? UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsBillable { get; init; }
}

public class GetTimeEntriesQueryHandler : IRequestHandler<GetTimeEntriesQuery, IList<EnhancedTimeEntryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public GetTimeEntriesQueryHandler(IApplicationDbContext context, ITenantService tenantService, IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<IList<EnhancedTimeEntryDto>> Handle(GetTimeEntriesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var query = _context.TimeEntries
            .Where(te => te.TenantId == tenantId)
            .Include(te => te.Ticket)
            .ThenInclude(t => t.Client)
            .AsQueryable();

        if (request.TicketId.HasValue)
            query = query.Where(te => te.TicketId == request.TicketId.Value);

        if (!string.IsNullOrEmpty(request.UserId))
            query = query.Where(te => te.UserId == request.UserId);

        if (request.StartDate.HasValue)
            query = query.Where(te => te.StartTime >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(te => te.StartTime <= request.EndDate.Value);

        if (request.IsBillable.HasValue)
            query = query.Where(te => te.IsBillable == request.IsBillable.Value);

        return await query
            .OrderByDescending(te => te.StartTime)
            .ProjectToListAsync<EnhancedTimeEntryDto>(_mapper.ConfigurationProvider, cancellationToken);
    }
}