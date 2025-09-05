using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Application.Clients.Queries.GetClients;

namespace ClientTicketingSaaS.Application.Clients.Queries.GetClient;

public record GetClientQuery(int Id) : IRequest<ClientDto>;

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public GetClientQueryHandler(IApplicationDbContext context, ITenantService tenantService, IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<ClientDto> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var client = await _context.Clients
            .Where(c => c.Id == request.Id && c.TenantId == tenantId)
            .ProjectTo<ClientDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, client);

        return client;
    }
}