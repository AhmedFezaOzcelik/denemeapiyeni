using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Models;
using Enoca.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Enoca.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CategoryController:Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Enoca.Models.Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            return Ok(categories);

        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Enoca.Models.Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();
            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpGet("pokemon/{CategoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemonByCategoryId(int categoryId) 
        {
            var pokemons= _mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonByCategories(categoryId));

            if(!ModelState.IsValid)
                return BadRequest();
            return Ok(pokemons);
        }

        //Bu metot HTTP post isteklerini karşılar.
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        //Başarılı ekleme durumunda 201 Created ve CategoryDto tipinde veri döner.
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        //Hatalı istek durumunda 400 bad request döner.
        [ProducesResponseType(400)]
        //Aynı isimde katagori varsa 409 conflict döner.
        [ProducesResponseType(409)]
        public IActionResult CreateCategory([FromBody] CategoryDto categorydto)
        {
            //Gönderilen veri boşsa hata döndürür.
            if (categorydto == null)
                return BadRequest();

            //Aynı isimde kategori olup olmadığını kontrol eder.
            var categories = _categoryRepository.GetCategories();
            if (categories.Any(c => c.Name.Trim().ToLower() == categorydto.Name.Trim().ToLower()))
                return Conflict("Bu isimde bir kategori var zaten.");

            //Dto dan Category modeline dönüştürür.
            var category = _mapper.Map<Category>(categorydto);

            //Veritabanına ekler.
            _categoryRepository.AddCategory(category);

            //Eklenen veriyi dto ya çevirip döndürür.
            var createdCategory = _mapper.Map<CategoryDto>(category);
            return CreatedAtAction(nameof(GetCategory), new { categoryId = createdCategory.Id }, createdCategory);  
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin,Manager")]
        //Swagger için olası cevap durumu kodlarını belirliyoruz.
        [ProducesResponseType(204)] //Başarılı içerik dönmüyor
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı

        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto updatedcategory)
        {
            //Fluent validation kontrolü 
            //Gelen updatedCategory verisinin , CategoryDtoValidator daki kurallara uyup uymadığını kontrol eder.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //2.Kayıt Mevcut mu kontrolü 
            //Güncellenmek istenen kategori veritabanında var olup olmadığını kontrol eder.
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound("Güncellenmek istenen kategori bulunamadı.");

            //Önce güncellenecek olan kategoriyi veritabanından alıyoruz
            var categoryToUpdate = _categoryRepository.GetCategory(categoryId);

            //AutoMapper ile , gelen DTO daki verileri mevcut nesnenin üzerine yazıyoruz 
            _mapper.Map(updatedcategory, categoryToUpdate);

            //Repository üzerinden güncelleme işlemini yapıyoruz
            if (!_categoryRepository.UpdateCategory(categoryToUpdate))
            {
                //Eğer kaydetme başarısız ise bir sunucu hatası döndürüyoruz
                ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu ");
                return StatusCode(500, ModelState);
            }


            return NoContent(); //Başarılı güncelleme sonrası 204 No Content döndürüyoruz


        }
        [HttpDelete("{categoryId}")]
        [Authorize  (Roles ="Admin")]
        [ProducesResponseType(204)] //Başarılı ve içerik dönmüyor.
        [ProducesResponseType(400)] //Hatalı istek
        [ProducesResponseType(404)] //Kaynak bulunamadı

        public IActionResult DeleteCategory(int categoryId)
        {
            //silinmek istenen kategorinin veritabanında var olup olmadığını kontrol et.
            if(!_categoryRepository.CategoryExists(categoryId))
                return NotFound("Silinmek istenen kategori bulunamadı.");
            //SİLME İŞLEMİ
            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            //Repository üzerinden silme işlemini gerçekleştiriyoruz.
            if(!_categoryRepository.DeleteCategory(categoryToDelete))
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
