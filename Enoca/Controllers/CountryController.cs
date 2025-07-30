using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Models;
using Enoca.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository; // Repository referansı
        private readonly IMapper _mapper; // AutoMapper referansı

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository; // Repository'i constructor ile alır
            _mapper = mapper; // Mapper'ı constructor ile alır
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries()); // Tüm ülkeleri DTO'ya çevirir
            return Ok(countries); // Sonucu döner
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(country);
        }

        [HttpGet("owners/{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]

        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            if (!_countryRepository.CountryExists(ownerId))
                return NotFound();
            var country = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(201, Type = typeof(CountryDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]

        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
                return BadRequest();
            var countries = _countryRepository.GetCountries();
            if (countries.Any(c => c.Name.Trim().ToLower() == countryCreate.Name.Trim().ToLower()))
                return Conflict("Bu isimde bir Ülke var zaten.");

            
            var country = _mapper.Map<Country>(countryCreate);

            
            _countryRepository.AddCountry(country);

            
            var createdCountry = _mapper.Map<CountryDto>(country);
            return CreatedAtAction(nameof(GetCountry), new { countryId = createdCountry.Id }, createdCountry);



        }
    }
}
