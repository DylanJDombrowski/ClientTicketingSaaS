using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Tickets.Commands.CreateTicket;

public record CreateTicketCommand : IRequest<int>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int ClientId { get; init; }
    public TicketPriority Priority { get; init; } = TicketPriority.Medium;
    public DateTime? DueDate { get; init; }
    public int EstimatedHours { get; init; }
}

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public CreateTicketCommandValidator(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(v => v.ClientId)
            .NotEmpty()
            .MustAsync(ClientExists)
                .WithMessage("Selected client does not exist or is not active.");

        RuleFor(v => v.EstimatedHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.DueDate)
            .GreaterThan(DateTime.Now)
            .When(v => v.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }

    private async Task<bool> ClientExists(int clientId, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        return await _context.Clients
            .AnyAsync(c => c.Id == clientId && c.TenantId == tenantId && c.IsActive, cancellationToken);
    }
}

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public CreateTicketCommandHandler(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<int> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = new Ticket
        {
            TenantId = tenantId,
            Title = request.Title,
            Description = request.Description,
            ClientId = request.ClientId,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
            ActualHours = 0
        };

        _context.Tickets.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}