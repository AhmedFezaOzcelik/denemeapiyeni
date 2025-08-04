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
        [HttpPut("{countryId}")]
        [Authorize(Roles = "Admin,Manager")]
        //Swagger için olası cevap durumu kodlarını belirliyoruz.
        [ProducesResponseType(204)] //Başarılı içerik dönmüyor
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı

        public IActionResult UpdateCategory(int countryId, [FromBody] CountryDto updatedcountry)
        {
            //Fluent validation kontrolü 
            //Gelen updatedCategory verisinin , CategoryDtoValidator daki kurallara uyup uymadığını kontrol eder.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.Kayıt Mevcut mu kontrolü 
            //Güncellenmek istenen kategori veritabanında var olup olmadığını kontrol eder.
            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Güncellenmek istenen country bulunamadı.");

            //Önce güncellenecek olan kategoriyi veritabanından alıyoruz
            var countryToUpdate = _countryRepository.GetCountry(countryId);

            //AutoMapper ile , gelen DTO daki verileri mevcut nesnenin üzerine yazıyoruz 
            _mapper.Map(updatedcountry, countryToUpdate);

            //Repository üzerinden güncelleme işlemini yapıyoruz
            if (!_countryRepository.UpdateCountry(countryToUpdate))
            {
                //Eğer kaydetme başarısız ise bir sunucu hatası döndürüyoruz
                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu ");
                return StatusCode(500, ModelState);
            }


            return NoContent(); //Başarılı güncelleme sonrası 204 No Content döndürüyoruz


        }
        [HttpDelete("{countryId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)] //Başarılı ve içerik dönmüyor.
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı

        public IActionResult DeleteCountry(int countryId)
        {
            //silinmek istenen kategorinin veritabanında var olup olmadığını kontrol et.
            if (!_countryRepository.CountryExists(countryId))
                return NotFound("Silinmek istenen country bulunamadı.");
            //SİLME İŞLEMİ
            var countryToDelete = _countryRepository.GetCountry(countryId);

            //Repository üzerinden silme işlemini gerçekleştiriyoruz.
            if (!_countryRepository.DeleteCountry(countryToDelete))
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
