using ClientTicketingSaaS.Application.Common.DTOs;
using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Tickets.Queries.GetTickets;

public class TicketDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TicketStatus Status { get; init; }
    public TicketPriority Priority { get; init; }
    
    public int ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string? ClientCompany { get; init; }
    
    public string? AssignedToId { get; init; }
    public DateTime? DueDate { get; init; }
    public int EstimatedHours { get; init; }
    public int ActualHours { get; init; }
    
    public DateTimeOffset Created { get; init; }
    public DateTimeOffset LastModified { get; init; }
    
    // Navigation properties for detailed view (will be empty in list view)
    public IList<TicketCommentDto> Comments { get; init; } = new List<TicketCommentDto>();
    public IList<TimeEntryDto> TimeEntries { get; init; } = new List<TimeEntryDto>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                .ForMember(dest => dest.ClientCompany, opt => opt.MapFrom(src => src.Client.Company))
                .ForMember(dest => dest.ActualHours, opt => opt.MapFrom(src => 
                    (int)src.TimeEntries.Sum(te => te.Hours)));
        }
    }
}