using AutoMapper;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            CreateMap<Models.Platform, Dto.PlatformReadDto>();
            CreateMap<Dto.PlatformCreateDto, Models.Platform>();
        }
    }
}