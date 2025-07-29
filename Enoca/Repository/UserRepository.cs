using Enoca.Data;
using Enoca.Models;
using Enoca.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Enoca.Repository
{
    public class UserRepository : IUserRepository
    {
        //Veritabanı ile iletişim kurmamızı sağlayan DataContext nesnesi.
        private readonly DataContext _context;

        //Constructer:Bu sınıf oluşturulduğunda ,Dependecy Injection sistemi
        //otomatik olarak bir DataContext nesnesini bu metoda parametre olarak verir.
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        //Kullanıcı adına göre veritabanından kullanıcı alan metot.
        public UserModel GetUserByUsername(string username)
        {

            //Users tablosunda,gönderilen "username" ile eşleşen ilk kaydı bulur ve döndürür.
            return _context.Users.Include(u=>u.Role).FirstOrDefault(u=> u.Username == username);
        }

        //Yeni bir kullanıcıyı varitabanınna ekleyen metot.
        public UserModel RegisterUser(UserModel user)
        {
            //Gelen "user" nesnesini Users tablosunda eklemek üzere işaretler.
            _context.Users.Add(user);
            //Değişiklikleri kaydeder ve kullanıcıya geri dödürür.
            return user;
        }

        //Veritabanında yapılan değişiklikleri kalıcı hale getiren metot.
        public bool Save()
        {
            //_context.SaveChanges() metodu,yapılan değişilik sayısını döndürür.
            var saved = _context.SaveChanges();
            return saved > 0; //Eğer değişiklik sayısı 0'dan büyükse, işlemler başarılı demektir.
        }

        //Belirtilen kullanıcı adının veritabanında zaten var olup olmadığını kontrol eden metot.
        public bool UserExists(string username)
        {
            //Users tablosunda,gönderilen "username" ile eşleşen herhangi bir kayıt var mı diye kontrol eder.
            return _context.Users.Any(u=>u.Username == username);

        }
    }
}
