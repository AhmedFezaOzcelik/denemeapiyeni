using Enoca.Dto;
using FluentValidation;
namespace Enoca.Validator
{
    //Bu sınıf ,AuthDto nesnesinin doğrulama kurallarını içerir.
    //AbstractValidator<AuthDto> sınıfından kalıtım alarak fluent validation özelliklerini kullanılır.
    public class AuthDtoValidator: AbstractValidator<AuthDto>
    {
        //Constructer bu bu metodun içinde tanımlanır.
        public AuthDtoValidator()
        {
            //Username için olacak kurallar.
            //Rulefor metodu,ahngi özellik için kural yazacağımızı belirtir.
            RuleFor(x => x.Username)
                //Şifrenin boş olmayacağını belirtiyoruz.
                .NotEmpty().WithMessage("Kullanıcıadı boş olamaz.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                //Matches kuralı şifrenin belirli bir desene uyması gerektiğini belirtir.
                //Bu desen en az bir büyük harf veya küçük harf içermediğini kontrol eder.
                .Matches("[A-Za-z]").WithMessage("Şifre en az bir harf içermelidir.")
                //Bu desen en az bir rakam içermediğini kontrol eder.
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
                //Bu desen, belirtilen özel karakterlerden  en az birini içerip içermdeğini kontrol eder.
                .Matches(@"[/*\-!@#$%^&*()_+={}\[\]:;""<>?,.|\\]").WithMessage("Şifre en az bir özel karakter içermelidir.");



        }

    }
}
