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

        }
    }
}
