using ClientTicketingSaaS.Domain.Entities;

namespace ClientTicketingSaaS.Application.Common.DTOs;

public class TicketCommentDto
{
    public int Id { get; init; }
    public string Comment { get; init; } = string.Empty;
    public bool IsInternal { get; init; }
    public DateTimeOffset Created { get; init; }
    public string? CreatedBy { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TicketComment, TicketCommentDto>();
        }
    }
}
