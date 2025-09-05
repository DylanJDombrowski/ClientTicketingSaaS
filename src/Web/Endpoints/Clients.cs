using ClientTicketingSaaS.Application.Clients.Commands.CreateClient;
using ClientTicketingSaaS.Application.Clients.Commands.UpdateClient;
using ClientTicketingSaaS.Application.Clients.Commands.DeleteClient;
using ClientTicketingSaaS.Application.Clients.Queries.GetClients;
using ClientTicketingSaaS.Application.Clients.Queries.GetClient;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Clients : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetClients);
        groupBuilder.MapPost(CreateClient);
        groupBuilder.MapGet(GetClient, "{id}");
        groupBuilder.MapPut(UpdateClient, "{id}");
        groupBuilder.MapDelete(DeleteClient, "{id}");
    }

    public async Task<IResult> GetClients(ISender sender)
    {
        var query = new GetClientsQuery();
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> GetClient(ISender sender, int id)
    {
        var query = new GetClientQuery(id);
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateClient(ISender sender, CreateClientCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/clients/{id}", new { Id = id });
    }

    public async Task<IResult> UpdateClient(ISender sender, int id, UpdateClientCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest("ID mismatch");

        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteClient(ISender sender, int id)
    {
        await sender.Send(new DeleteClientCommand(id));
        return Results.NoContent();
    }
}