using AutoMapper;
using Stripe.Database.Entities;
using Stripe.Dto;

namespace Stripe.Mapping;

public class FromCreationModelToAppUserMappingProfile : Profile
{
    public FromCreationModelToAppUserMappingProfile()
    {
        CreateMap<RegistrationDto, UserEntity>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}