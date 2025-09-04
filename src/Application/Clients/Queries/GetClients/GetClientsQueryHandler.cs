using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Application.Common.Mappings;

namespace ClientTicketingSaaS.Application.Clients.Queries.GetClients;

public record GetClientsQuery : IRequest<IList<ClientDto>>;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IList<ClientDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IMapper _mapper;

    public GetClientsQueryHandler(IApplicationDbContext context, ITenantService tenantService, IMapper mapper)
    {
        _context = context;
        _tenantService = tenantService;
        _mapper = mapper;
    }

    public async Task<IList<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        return await _context.Clients
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .OrderBy(c => c.Name)
            .ProjectToListAsync<ClientDto>(_mapper.ConfigurationProvider, cancellationToken);
    }
}

public class ClientDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Company { get; init; }
    public bool IsActive { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Client, ClientDto>();
        }
    }
}