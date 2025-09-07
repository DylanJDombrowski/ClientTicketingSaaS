using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Common.DTOs;

public class EnhancedTimeEntryDto : TimeEntryDto
{
    public string TicketTitle { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public TicketStatus TicketStatus { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TimeEntry, EnhancedTimeEntryDto>()
                .ForMember(dest => dest.TicketTitle, opt => opt.MapFrom(src => src.Ticket.Title))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Ticket.Client.Name))
                .ForMember(dest => dest.TicketStatus, opt => opt.MapFrom(src => src.Ticket.Status));
        }
    }
}