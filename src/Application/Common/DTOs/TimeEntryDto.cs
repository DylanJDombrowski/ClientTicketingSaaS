using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Common.DTOs;

public class TimeEntryDto
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Hours { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public bool IsBillable { get; init; }
    public DateTimeOffset Created { get; init; }
    public string? CreatedBy { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TimeEntry, TimeEntryDto>();
        }
    }
}