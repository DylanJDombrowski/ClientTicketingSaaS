using ClientTicketingSaaS.Application.Tickets.Commands.CreateTicket;
using ClientTicketingSaaS.Application.Tickets.Commands.UpdateTicket;
using ClientTicketingSaaS.Application.Tickets.Commands.DeleteTicket;
using ClientTicketingSaaS.Application.Tickets.Queries.GetTickets;
using ClientTicketingSaaS.Application.Tickets.Queries.GetTicket;
using ClientTicketingSaaS.Domain.Enums;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Tickets : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetTickets).RequireAuthorization();
        groupBuilder.MapPost(CreateTicket).RequireAuthorization();
        groupBuilder.MapGet(GetTicket, "{id}").RequireAuthorization();
        groupBuilder.MapPut(UpdateTicket, "{id}").RequireAuthorization();
        groupBuilder.MapDelete(DeleteTicket, "{id}").RequireAuthorization();
        
        // Fixed PATCH mapping - removed the pattern parameter issue
        groupBuilder.MapPatch("{id}/status", UpdateTicketStatus).RequireAuthorization();
    }

    public async Task<IResult> GetTickets(
        ISender sender,
        TicketStatus? status = null,
        TicketPriority? priority = null,
        int? clientId = null,
        string? assignedToId = null)
    {
        var query = new GetTicketsQuery
        {
            Status = status,
            Priority = priority,
            ClientId = clientId,
            AssignedToId = assignedToId
        };

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetTicket(ISender sender, int id)
    {
        try
        {
            var query = new GetTicketQuery(id);
            var result = await sender.Send(query);
            return Results.Ok(result);
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Ticket with ID {id} not found");
        }
    }

    public async Task<IResult> CreateTicket(ISender sender, CreateTicketRequest request)
    {
        try
        {
            var command = new CreateTicketCommand
            {
                Title = request.Title,
                Description = request.Description,
                ClientId = request.ClientId,
                Priority = request.Priority,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours
            };

            var id = await sender.Send(command);
            return Results.Created($"/api/tickets/{id}", new { Id = id });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { Errors = ex.Errors });
        }
    }

    public async Task<IResult> UpdateTicket(ISender sender, int id, UpdateTicketRequest request)
    {
        try
        {
            var command = new UpdateTicketCommand
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                AssignedToId = request.AssignedToId,
                DueDate = request.DueDate,
                EstimatedHours = request.EstimatedHours
            };

            await sender.Send(command);
            return Results.NoContent();
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Ticket with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { Errors = ex.Errors });
        }
    }

    public async Task<IResult> UpdateTicketStatus(ISender sender, int id, UpdateTicketStatusRequest request)
    {
        try
        {
            // We can reuse the UpdateTicketCommand but only set the status
            var query = new GetTicketQuery(id);
            var ticket = await sender.Send(query);
            
            var command = new UpdateTicketCommand
            {
                Id = id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = request.Status,
                Priority = ticket.Priority,
                AssignedToId = ticket.AssignedToId,
                DueDate = ticket.DueDate,
                EstimatedHours = ticket.EstimatedHours
            };

            await sender.Send(command);
            return Results.NoContent();
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Ticket with ID {id} not found");
        }
    }

    public async Task<IResult> DeleteTicket(ISender sender, int id)
    {
        try
        {
            await sender.Send(new DeleteTicketCommand(id));
            return Results.NoContent();
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Ticket with ID {id} not found");
        }
    }
}

// ===== Request DTOs =====

public record CreateTicketRequest(
    string Title,
    string Description,
    int ClientId,
    TicketPriority Priority = TicketPriority.Medium,
    DateTime? DueDate = null,
    int EstimatedHours = 0
);

public record UpdateTicketRequest(
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    string? AssignedToId,
    DateTime? DueDate,
    int EstimatedHours
);

public record UpdateTicketStatusRequest(TicketStatus Status);