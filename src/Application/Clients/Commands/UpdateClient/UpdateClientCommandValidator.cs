using ClientTicketingSaaS.Application.Common.Interfaces;

namespace ClientTicketingSaaS.Application.Clients.Commands.UpdateClient;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantService _tenantService;

    public UpdateClientCommandValidator(IApplicationDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254)
            .MustAsync(BeUniqueEmail)
                .WithMessage("'{PropertyName}' must be unique within your organization.")
                .WithErrorCode("Unique");

        RuleFor(v => v.Phone)
            .MaximumLength(50);

        RuleFor(v => v.Company)
            .MaximumLength(200);
    }

    private async Task<bool> BeUniqueEmail(UpdateClientCommand model, string email, CancellationToken cancellationToken)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        
        return !await _context.Clients
            .AnyAsync(c => c.TenantId == tenantId && c.Email == email && c.Id != model.Id, cancellationToken);
    }
}