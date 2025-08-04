using AutoMapper;
using Enoca.Models;
using Enoca.Dto;

namespace Enoca.Helper
{
    public class MappingProfile:Profile 
    {
        public MappingProfile()
        {
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Country, CountryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CountryDto, Country>();
            CreateMap<PokemonDto, Pokemon>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();
            CreateMap<RoleModel, RoleDto>();
            
            //User dan user dto ya maplerken ,Role nesenesinin Name ini RoleName a ata.
            CreateMap<UserModel, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

        }
    }
}
