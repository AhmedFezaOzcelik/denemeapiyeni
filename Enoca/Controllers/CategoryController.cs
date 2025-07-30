using AutoMapper;
using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Models;
using Enoca.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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



    }
}
