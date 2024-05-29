using AutoMapper;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Models;

namespace PlatformService
{
    public static class PlatformEndpointsSetup
    {
        public static void MapEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            MapGetPlatforms(routeBuilder);
            MapGetPlatformById(routeBuilder);
            MapCreatePlatform(routeBuilder);
        }

        private static void MapCreatePlatform(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/platforms", 
                async (PlatformCreateDto platformCreateDto, IPlatformRepository platformRepository, IMapper mapper, IMessageBusClient messageBusClient) =>
            {
                var platform = mapper.Map<Platform>(platformCreateDto);
                platformRepository.CreatePlatform(platform);
                platformRepository.SaveChanges();
                var platformReadDto = mapper.Map<PlatformReadDto>(platform);

                try
                {
                    var platformPublishedDto = mapper.Map<PlatformPublishDto>(platformReadDto);
                    platformPublishedDto.Event = "Platform_Published";
                    messageBusClient.PublishNewPlatform(platformPublishedDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not publish to message bus: {ex.Message}");
                }

                return Results.Created($"/platforms/{platformReadDto.Id}", platformReadDto);
            });
        }

        private static void MapGetPlatformById(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/platforms/{id}", 
                (int id, IPlatformRepository platformRepository, IMapper mapper) =>
            {
                var platform = platformRepository.GetPlatformById(id);
                if (platform is null)
                {
                    return Results.NotFound();
                }

                var platformDto = mapper.Map<PlatformReadDto>(platform);
                return Results.Ok(platformDto);
            })
            .WithName("GetPlatformById")
            .WithOpenApi();
        }

        private static void MapGetPlatforms(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/platforms", 
                (IPlatformRepository platformRepository, IMapper mapper) =>
            {
                var platforms = platformRepository.GetAllPlatforms();
                var platformsDto = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
                return Results.Ok(platformsDto);
            })
            .WithName("GetPlatforms")
            .WithOpenApi();
        }
    }
}