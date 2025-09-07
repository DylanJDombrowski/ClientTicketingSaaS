using ClientTicketingSaaS.Application.Common.Interfaces;
using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.TimeEntries.Commands.CreateTimeEntry;

public record CreateTimeEntryCommand : IRequest<int>
{
    public int TicketId { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Hours { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public bool IsBillable { get; init; } = true;
}

public class CreateTimeEntryCommandValidator : AbstractValidator<CreateTimeEntryCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public CreateTimeEntryCommandValidator(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(v => v.Hours)
            .GreaterThan(0)
            .LessThanOrEqualTo(24)
            .WithMessage("Hours must be between 0.1 and 24");

        RuleFor(v => v.TicketId)
            .NotEmpty()
            .MustAsync(TicketExists)
                .WithMessage("Selected ticket does not exist.");

        RuleFor(v => v.EndTime)
            .GreaterThan(v => v.StartTime)
            .When(v => v.EndTime.HasValue)
            .WithMessage("End time must be after start time.");
    }

    private async Task<bool> TicketExists(int ticketId, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        return await _context.Tickets
            .AnyAsync(t => t.Id == ticketId && t.TenantId == tenantId, cancellationToken);
    }
}

public class CreateTimeEntryCommandHandler : IRequestHandler<CreateTimeEntryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly IUser _currentUser;

    public CreateTimeEntryCommandHandler(IApplicationDbContext context, ITenantService tenantService, IUser currentUser)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateTimeEntryCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();

        var entity = new TimeEntry
        {
            TenantId = tenantId,
            TicketId = request.TicketId,
            UserId = _currentUser.Id!,
            Description = request.Description,
            Hours = request.Hours,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsBillable = request.IsBillable
        };

        _context.TimeEntries.Add(entity);

        // Update ticket actual hours
        var ticket = await _context.Tickets.FindAsync(request.TicketId, cancellationToken);
        if (ticket != null)
        {
            var totalHours = await _context.TimeEntries
                .Where(te => te.TicketId == request.TicketId)
                .SumAsync(te => te.Hours, cancellationToken);
            
            ticket.ActualHours = (int)(totalHours + request.Hours);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}