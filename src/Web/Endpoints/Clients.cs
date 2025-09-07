using ClientTicketingSaaS.Application.Clients.Commands.CreateClient;
using ClientTicketingSaaS.Application.Clients.Commands.UpdateClient;
using ClientTicketingSaaS.Application.Clients.Commands.DeleteClient;
using ClientTicketingSaaS.Application.Clients.Queries.GetClients;
using ClientTicketingSaaS.Application.Clients.Queries.GetClient;
using ClientTicketingSaaS.Application.Common.Exceptions;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Clients : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetClients).RequireAuthorization();
        groupBuilder.MapPost(CreateClient).RequireAuthorization();
        groupBuilder.MapGet(GetClient, "{id}").RequireAuthorization();
        groupBuilder.MapPut(UpdateClient, "{id}").RequireAuthorization();
        groupBuilder.MapDelete(DeleteClient, "{id}").RequireAuthorization();
    }

    public async Task<IResult> GetClients(ISender sender)
    {
        var query = new GetClientsQuery();
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetClient(ISender sender, int id)
    {
        try
        {
            var query = new GetClientQuery(id);
            var result = await sender.Send(query);
            return Results.Ok(result);
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Client with ID {id} not found");
        }
    }

    public async Task<IResult> CreateClient(ISender sender, CreateClientCommand command)
    {
        try
        {
            var id = await sender.Send(command);
            return Results.Created($"/api/clients/{id}", new { Id = id });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { Errors = ex.Errors });
        }
    }

    public async Task<IResult> UpdateClient(ISender sender, int id, UpdateClientRequest request)
    {
        try
        {
            var command = new UpdateClientCommand
            {
                Id = id,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Company = request.Company,
                IsActive = request.IsActive
            };

            await sender.Send(command);
            return Results.NoContent();
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Client with ID {id} not found");
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new { Errors = ex.Errors });
        }
    }

    public async Task<IResult> DeleteClient(ISender sender, int id)
    {
        try
        {
            await sender.Send(new DeleteClientCommand(id));
            return Results.NoContent();
        }
        catch (Application.Common.Exceptions.NotFoundException)
        {
            return Results.NotFound($"Client with ID {id} not found");
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { Message = ex.Message });
        }
    }
}

// Request DTOs for the API
public record UpdateClientRequest(
    string Name,
    string Email,
    string? Phone,
    string? Company,
    bool IsActive
);