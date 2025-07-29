namespace Enoca.Models
{
    //Bu sınıf sisteme kayıtlı olan her bir kullanıcıyı temsil eder.
    public class UserModel
    {
        //Her kullanıcı için benzersiz bir kimlik numarası.
        public int Id { get; set; }
        //Kullanıcın sisteme giriş yaparken kullanacağı kullanıcı adı.
        public string Username { get; set; }

        //Bu iki alan ,şifrenin güvenli bir şekilde haslenmiş halini saklamak için kullanılır.
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        //Bu kullanıcının sahip olduğu rolün kimlik numarasıdır.
        public int RoleId { get; set; }

        //Entity Fraemwork Core un User ile Role arasında bir ilişki olduğunu anlmasını sağlar.

        public RoleModel  Role  { get; set; }

    }
}
