using AutoMapper;
using Enoca.Data;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Models;
using Enoca.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonrepository;
        private readonly DataContext context;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonrepository, DataContext context,IMapper mapper)
        {
            _pokemonrepository = pokemonrepository;
            this.context = context;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Enoca.Models.Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _pokemonrepository.GetPokemons();
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            return Ok(pokemons);

        }
        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonrepository.PokemonExists(pokeId))
                return NotFound();
            var pokemon =_mapper.Map<PokemonDto>(_pokemonrepository.GetPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }
        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            var rating = _pokemonrepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(rating);
        }

        
        [HttpPost]
        
        [ProducesResponseType(201, Type = typeof(PokemonDto))]
        
        [ProducesResponseType(400)]
        
        [ProducesResponseType(409)]
        public IActionResult CreateCategory([FromBody] PokemonDto pokemondto)
        {
            
            if (pokemondto == null)
                return BadRequest();

            
            var pokemons = _pokemonrepository.GetPokemons();
            if (pokemons.Any(c => c.Name.Trim().ToLower() == pokemondto.Name.Trim().ToLower()))
                return Conflict("Bu isimde bir pokemon var zaten.");

            
            var pokemon = _mapper.Map<Pokemon>(pokemondto);

            
            _pokemonrepository.AddPokemon(pokemon);

            
            var pokemonDto = _mapper.Map<PokemonDto>(pokemon);
            return CreatedAtAction(nameof(GetPokemon), new { pokeId = pokemondto.Id }, pokemondto);
        }
    } 

}
