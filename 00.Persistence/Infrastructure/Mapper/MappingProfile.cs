using AutoMapper;
using Domain.Entity;
using Infrastructure.Dto;

namespace Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}
