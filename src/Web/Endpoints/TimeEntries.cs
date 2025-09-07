using ClientTicketingSaaS.Application.TimeEntries.Commands.CreateTimeEntry;
using ClientTicketingSaaS.Application.TimeEntries.Queries.GetTimeEntries;

namespace ClientTicketingSaaS.Web.Endpoints;

public class TimeEntries : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetTimeEntries).RequireAuthorization();
        groupBuilder.MapPost(CreateTimeEntry).RequireAuthorization();
        groupBuilder.MapGet(GetTimeEntry, "{id}").RequireAuthorization();
        groupBuilder.MapPut(UpdateTimeEntry, "{id}").RequireAuthorization();
        groupBuilder.MapDelete(DeleteTimeEntry, "{id}").RequireAuthorization();
        
        // Convenience endpoints
        groupBuilder.MapGet(GetTimeEntriesForTicket, "ticket/{ticketId}").RequireAuthorization();
        groupBuilder.MapGet(GetMyTimeEntries, "my").RequireAuthorization();
    }

    public async Task<IResult> GetTimeEntries(
        ISender sender,
        int? ticketId = null,
        string? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool? isBillable = null)
    {
        var query = new GetTimeEntriesQuery
        {
            TicketId = ticketId,
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            IsBillable = isBillable
        };

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetTimeEntriesForTicket(ISender sender, int ticketId)
    {
        var query = new GetTimeEntriesQuery { TicketId = ticketId };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetMyTimeEntries(ISender sender, IUser currentUser, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = new GetTimeEntriesQuery 
        { 
            UserId = currentUser.Id,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateTimeEntry(ISender sender, CreateTimeEntryRequest request)
    {
        try
        {
            var command = new CreateTimeEntryCommand
            {
                TicketId = request.TicketId,
                Description = request.Description,
                Hours = request.Hours,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsBillable = request.IsBillable
            };

            var id = await sender.Send(command);
            return Results.Created($"/api/time-entries/{id}", new { Id = id });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { Errors = ex.Errors });
        }
    }

    public Task<IResult> GetTimeEntry(ISender sender, int id)
    {
        // Placeholder - you can implement this later with a GetTimeEntryQuery
        return Task.FromResult(Results.Ok(new { Message = "GetTimeEntry not implemented yet" }));
    }

    public Task<IResult> UpdateTimeEntry(ISender sender, int id, UpdateTimeEntryRequest request)
    {
        // Placeholder - implement similar pattern to tickets
        return Task.FromResult(Results.Ok(new { Message = "UpdateTimeEntry not implemented yet" }));
    }

    public Task<IResult> DeleteTimeEntry(ISender sender, int id)
    {
        // Placeholder - implement similar pattern to tickets
        return Task.FromResult(Results.Ok(new { Message = "DeleteTimeEntry not implemented yet" }));
    }
}

// ===== REQUEST DTOS =====

public record CreateTimeEntryRequest(
    int TicketId,
    string Description,
    decimal Hours,
    DateTime StartTime,
    DateTime? EndTime = null,
    bool IsBillable = true
);

public record UpdateTimeEntryRequest(
    string Description,
    decimal Hours,
    DateTime StartTime,
    DateTime? EndTime,
    bool IsBillable
);