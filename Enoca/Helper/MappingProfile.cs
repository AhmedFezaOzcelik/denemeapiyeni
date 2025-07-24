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

        }
    }
}
