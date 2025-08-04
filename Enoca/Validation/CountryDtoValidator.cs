using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Repository;
using FluentValidation;
namespace Enoca.Validation
{
    public class CountryDtoValidator:AbstractValidator<CountryDto>
    {
        private readonly ICountryRepository _countryRepository;

        public CountryDtoValidator(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;

            RuleFor(co => co.Name)
                .NotEmpty().WithMessage("Counrty adı boş olamaz")
                .Length(-1, 15).WithMessage("Country ismi 15 karakterden fazla olamaz")
                .Must(BeUniqueCategoryName).WithMessage("Bu isimde bir Country zaten mevcut");
        }
        //Veritabanını kontrol eden bir metot.
        private bool BeUniqueCategoryName(string name)
        {
            //Eğer bu isimde bir pokemon yoksa geçerlidir true döner.
            //Eğer bu isimde bir pokemon varsa geçersizdir false döner.
            return !_countryRepository.CountryExistsByName(name);
        }
    }
}
