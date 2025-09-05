using ClientTicketingSaaS.Application.Clients.Commands.CreateClient;
using ClientTicketingSaaS.Application.Clients.Queries.GetClients;

namespace ClientTicketingSaaS.Web.Endpoints;

public class Clients : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetClients);
        groupBuilder.MapPost(CreateClient);
    }

    public async Task<IResult> GetClients(ISender sender)
    {
        var query = new GetClientsQuery();
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public async Task<IResult> CreateClient(ISender sender, CreateClientCommand command)
    {
        var id = await sender.Send(command);
        return Results.Created($"/clients/{id}", new { Id = id });
    }
}