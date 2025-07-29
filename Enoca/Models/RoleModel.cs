namespace Enoca.Models
{
    //Bu sınıf, sistemdeki kullanıcı rollerini temsil emek için kullanılır.
    public class RoleModel
    {
        //Her rol için benzersiz bir kimlik numarası.
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        //Bu role sahip olan kullanıcıların hedefi.

        //Bir rolün birden çok kullanıcısı olabilir.
        public ICollection<UserModel> Users { get; set; } 


    }
}
