using AutoMapper;
using CommandsService.Data;
using CommandsService.Dto;
using CommandsService.Models;

namespace CommandsService
{
    public static class CommandsEndpointsSetup
    {
        public static void MapEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/api/commands/platforms", (ICommandRepository commandRepository, IMapper mapper) =>
            {
                return Results.Ok("Inbound test of POST command endpoint in CommandsService");
            });

            routeBuilder.MapGet("/api/commands/platforms", (ICommandRepository commandRepository, IMapper mapper) =>
            {
                Console.WriteLine("--> Getting Platforms from CommandsService");

                var platforms = commandRepository.GetAllPlatforms();
                return Results.Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
            });

            routeBuilder.MapGet("/api/commands/platforms/{platformId}", (int platformId, ICommandRepository commandRepository, IMapper mapper) =>
            {
                Console.WriteLine($"--> Getting Commands for Platform: {platformId} from CommandsService");

                if (!commandRepository.PlatformExists(platformId))
                {
                    return Results.NotFound();
                }

                var commandsForPlatform = commandRepository.GetCommandsForPlatform(platformId);
                return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commandsForPlatform));
            });

            routeBuilder.MapPost("/api/commands/platforms/{platformId}", (int platformId, CommandCreateDto commandCreateDto, ICommandRepository commandRepository, IMapper mapper) =>
            {
                Console.WriteLine($"--> Adding Command to Platform: {platformId} from CommandsService");

                if (!commandRepository.PlatformExists(platformId))
                {
                    return Results.NotFound();
                }

                var command = mapper.Map<Command>(commandCreateDto);
                commandRepository.CreateCommand(platformId, command);
                commandRepository.SaveChanges();

                return Results.Created($"/api/commands/platforms/{platformId}/{command.Id}", mapper.Map<CommandReadDto>(command));
            });

            routeBuilder.MapGet("/api/commands/platforms/{platformId}/{commandId}", (int platformId, int commandId, ICommandRepository commandRepository, IMapper mapper) =>
            {
                Console.WriteLine($"--> Getting specific command for platform {platformId} and command {commandId} from CommandsService");

                if (!commandRepository.PlatformExists(platformId))
                {
                    return Results.NotFound();
                }

                var command = commandRepository.GetCommand(platformId, commandId);

                if (command == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(mapper.Map<CommandReadDto>(command));
            });
        }
    }
}