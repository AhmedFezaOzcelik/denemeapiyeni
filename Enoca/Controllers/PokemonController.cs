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

        //Sadece Admin ve Manager rolündekilerin erişebileceği endpoint
        
        [HttpPost]

        [Authorize(Roles = "Admin,Manager")]

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

        [HttpPut("{pokeId}")]
        [Authorize(Roles = "Admin,Manager")]
        //Swagger için olası cevap durumu kodlarını belirliyoruz.
        [ProducesResponseType(204)] //Başarılı içerik dönmüyor
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı
        
        public IActionResult UpdatePokemon(int pokeId , [FromBody] PokemonDto updatedpokemon)
        {
            //Fluent validation kontrolü 
            //Gelen updatedPokemon verisinin , PokemonDtoValidator daki kurallara uyup uymadığını kontrol eder.
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.Kayıt Mevcut mu kontrolü 
            //Güncellenmek istenen Pokemon un veritabanında var olup olmadığını kontrol eder.
            if(!_pokemonrepository.PokemonExists(pokeId))
                return NotFound("Güncellenmek istenen pokemon bulunamadı.");

            //Önce güncellenecek olan Pokemon u veritabanından alıyoruz
            var pokemonToUpdate=_pokemonrepository.GetPokemon(pokeId);

            //AutoMapper ile , gelen DTO daki verileri mevcut nesnenin üzerine yazıyoruz 
            _mapper.Map(updatedpokemon, pokemonToUpdate);

            //Repository üzerinden güncelleme işlemini yapıyoruz
            if(!_pokemonrepository.UpdatePokemon(pokemonToUpdate))
            {
                //Eğer kaydetme başarısız ise bir sunucu hatası döndürüyoruz
                ModelState.AddModelError("","Güncelleme sırasında bir hata oluştu ");
                return StatusCode(500, ModelState);
            }
            

            return NoContent(); //Başarılı güncelleme sonrası 204 No Content döndürüyoruz


        }
        [HttpDelete("{pokeId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)] //Başarılı ve içerik dönmüyor.
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı

        public IActionResult DeletePokemon(int pokeId)
        {
            //silinmek istenen kategorinin veritabanında var olup olmadığını kontrol et.
            if (!_pokemonrepository.PokemonExists(pokeId))
                return NotFound("Silinmek istenen pokemon bulunamadı.");
            //SİLME İŞLEMİ
            var pokemonToDelete = _pokemonrepository.GetPokemon(pokeId);

            //Repository üzerinden silme işlemini gerçekleştiriyoruz.
            if (!_pokemonrepository.DeletePokemon(pokemonToDelete))
            {
                //Eğer kaydetme işlemi başarısız olursa 
                //Sunucu hatası code 500 dödürüyoruz
                ModelState.AddModelError("", "Silme işlemi sırasında bir hata oluştu.");
                return StatusCode(500, ModelState);
            }
            //işlem başarılı olursa 204 No context durum kodunu döndürüyoruz
            //Bu kod işlem başarılı oldu sana döndürecek bişeyim yok demektir.

            return NoContent();
        }
    } 

}
