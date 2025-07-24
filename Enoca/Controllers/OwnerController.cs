using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class OwnerController:Controller
    {
        private readonly IOwnerRepository _ownerrepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerrepository,IMapper mapper)
        {
            _ownerrepository = ownerrepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Enoca.Models.Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _ownerrepository.GetOwners();
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            return Ok(owners);

        }
        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerrepository.OwnerExists(ownerId))
                return NotFound();
            var owner = _mapper.Map<OwnerDto>(_ownerrepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(owner);
        }
        [HttpGet("{ownerId}/pokemons")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
                       if (!_ownerrepository.OwnerExists(ownerId))
                return NotFound();
            var owner=_mapper.Map<List<PokemonDto>>(_ownerrepository.GetPokemonByOwner(ownerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);
        }

    }
}
