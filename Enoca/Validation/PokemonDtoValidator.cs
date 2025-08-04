using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Repository;
using FluentValidation;
namespace Enoca.Validation
{
    public class PokemonDtoValidator:AbstractValidator<PokemonDto>
    {
        private readonly IPokemonRepository _pokemonRepository;

        public PokemonDtoValidator(IPokemonRepository pokemonRepository)
        {
            _pokemonRepository = pokemonRepository;

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Pokemon adı boş olamaz")
                .Length(-1,15).WithMessage("Pokemon ismi 15 karakterden fazla olamaz")
                .Must(BeUniqueCategoryName).WithMessage("Bu isimde bir Pokemon zaten mevcut");
            RuleFor(p => p.Birthdate)
                .NotEmpty().WithMessage("Pokemon doğum tarihi boş olamaz")
                .LessThan(DateTime.Now).WithMessage("Pokemon doğum tarihi gelecekte olamaz");
        }
        //Veritabanını kontrol eden bir metot.
        private bool BeUniqueCategoryName(string name)
        {
            //Eğer bu isimde bir pokemon yoksa geçerlidir true döner.
            //Eğer bu isimde bir pokemon varsa geçersizdir false döner.
            return !_pokemonRepository.PokemonExistsByName(name);
        }
    }
}
