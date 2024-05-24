namespace CommandsService
{
    public static class CommandsEndpointsSetup
    {
        public static void MapEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/api/commands/platforms", () =>
            {
                return Results.Ok("Inbound test of POST command endpoint in CommandsService");
            });
        }
    }
}