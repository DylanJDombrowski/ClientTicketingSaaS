// Replace src/Application/Tickets/Commands/UpdateTicket/UpdateTicketCommand.cs

using ClientTicketingSaaS.Application.Common.Interfaces;

namespace ClientTicketingSaaS.Application.Tickets.Commands.UpdateTicket;

public record UpdateTicketCommand : IRequest
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TicketStatus Status { get; init; }
    public TicketPriority Priority { get; init; }
    public string? AssignedToId { get; init; }
    public DateTime? DueDate { get; init; }
    public int EstimatedHours { get; init; }
}

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(v => v.EstimatedHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.DueDate)
            .GreaterThan(DateTime.UtcNow) // Use UTC for comparison
            .When(v => v.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
    }
}

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public UpdateTicketCommandHandler(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = await _context.Tickets
            .Where(t => t.Id == request.Id && t.TenantId == tenantId)
            .FirstOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Status = request.Status;
        entity.Priority = request.Priority;
        entity.AssignedToId = request.AssignedToId;
        // Convert to UTC if DueDate is provided
        entity.DueDate = request.DueDate?.ToUniversalTime();
        entity.EstimatedHours = request.EstimatedHours;

        await _context.SaveChangesAsync(cancellationToken);
    }
}