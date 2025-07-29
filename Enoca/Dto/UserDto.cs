namespace Enoca.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = String.Empty;

        //Kullanıcının rolünü de Dto içinde göstermek,
        //istemci tarafında yetkilendirme kontrolleri için faydalı olabilir.
        public string RoleName { get; set; } = string.Empty;
    }
}
