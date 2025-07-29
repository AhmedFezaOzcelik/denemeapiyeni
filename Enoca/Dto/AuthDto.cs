namespace Enoca.Dto
{
    // İç içe sınıf yapısını kaldırıyoruz.
    // Artık AuthDto'nun kendisi Username ve Password özelliklerine sahip.
    public class AuthDto
    {
        //Kullanıcının giriş yaparken kullanacağı kullanıcı adı
        public string Username { get; set; } = string.Empty;

        //Kullanıcının giriş yaparken kullanacağı şifre 
        public string Password { get; set; } = string.Empty;
    }
}
