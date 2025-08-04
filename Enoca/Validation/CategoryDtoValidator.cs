using Enoca.Dto;
using Enoca.Interfaces;
using FluentValidation;
namespace Enoca.Validation
{
    public class CategoryDtoValidator: AbstractValidator<CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryDtoValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .Length(-1, 15).WithMessage("katogori ismi 15 karakterden fazla olamaz")
                .Must(BeUniqueCategoryName).WithMessage("Bu isimde bir kategori zaten mevcut");     
        }

        //Veritabanını kontrol eden bir metot.
        private bool BeUniqueCategoryName(string name)
        {
            //Eğer bu isimde bir kategori yoksa geçerlidir true döner.
            //Eğer bu isimde bir kategori varsa geçersizdir false döner.
            return !_categoryRepository.CategoryExistsByName(name);
        }
    }
}
