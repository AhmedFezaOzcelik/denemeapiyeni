using Enoca.Models;
namespace Enoca.Interfaces
{
    //Bu arayüz,kullanıcılarla ilgili tüm veri tabanı işlemlerinin 
    //sözleşmesini tanımlar.
    public interface IUserRepository
    {

        //Belirtilen kullanıcı adına sahip kullanıcıyı,rol bilgisiyle birlikte,
        //veritabanından getirir login işlemi için kullanılır.
        UserModel GetUserByUsername(string username);

        //Belirtilen kullanıcı adının veritabanında mevcut olup olmadığını kontrol eder.
        //Yeni kullanıcı kaydı sırasında ,aynı kullanıcı adının tekrar  kullanılmasını engellemek amacıyla kullanılır
        bool UserExists(string username);
        //Yeni bir kullanıcıyı veri tabanına kaydeder.
        UserModel RegisterUser(UserModel user);

        // Değişiklikleri veritabanına kaydetmek için genel bir metot.
        // Bu, birden fazla işlemi tek seferde kaydetmek için esneklik sağlar.
        bool Save();
    }
}
