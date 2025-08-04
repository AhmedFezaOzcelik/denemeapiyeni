using Enoca.Dto;
using Enoca.Interfaces;
using Enoca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Enoca.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // --- YENİ KULLANICI KAYIT ENDPOINT'İ ---
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthDto authDto)
        {
            //Gelen verinin,AuthDtoValidator daki kurallara uyup uymadığını kontrol et.
            //Fluent Validation bu kontrol öncesinde modelstate i otomatik olarak doldurur.
            if (!ModelState.IsValid)
            {
                //En az bir doğrulama hatası varsa ,işlemi devam ettirme.
                //BadRequest(ModelState) ile bu hataları döndür.
                return BadRequest(ModelState);
            }
            // Gönderilen kullanıcı adının zaten alınıp alınmadığını kontrol et
            if (_userRepository.UserExists(authDto.Username)) 
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }
            //2.Gelen kullanıcı adına göre hangi rolün atanacağı belirleniyor.
            int roleIdToAssign;
            //Admin olarak kullanılacak kullanıcı adlarını belirliyorum 
            var adminUsernames = new List<string> { "admin", "feza", "feza1","feza2", "feza3","ahmed","ahmed1","ahmed2","ahmed3" };

            
            if(adminUsernames.Contains(authDto.Username.ToLower()))
            {
                //Eğer gelen kullanıcı adı listede varsa 
                //Ona Admin rolünün Id sini ata yani 1 i ata.
                roleIdToAssign = 1; 
            }
            else
            {
                //Eğer gelen kullanıcı adı listede yoksa 
                //Ona User rolünün Id sini ata yani 2 yi ata.
                roleIdToAssign = 2; 
            }

            // Güvenli şifreleme için hash ve salt oluştur.
            CreatePasswordHash(authDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Yeni kullanıcı nesnesi oluştur.
            var userToCreate = new UserModel
            {
                Username = authDto.Username, // "Username" olarak düzeltildi
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = roleIdToAssign
            };

            _userRepository.RegisterUser(userToCreate);
            _userRepository.Save();

            return Ok("Kullanıcı Başarıyla Oluşturuldu");
        } // <-- Register metodu burada BİTİYOR

        // --- GİRİŞ (LOGIN) ENDPOINT'İ (AYRI BİR METOT OLARAK) ---
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthDto authDto)
        {
            //Gelen verinin,AuthDtoValidator daki kurallara uyup uymadığını kontrol et.
            //Fluent Validation bu kontrol öncesinde modelstate i otomatik olarak doldurur.
            if (!ModelState.IsValid)
            {
                //En az bir doğrulama hatası varsa ,işlemi devam ettirme.
                //BadRequest(ModelState) ile bu hataları döndür.
                return BadRequest(ModelState);
            }
            // Kullanıcıyı veritabanından kullanıcı adına göre bul.
            var userFromRepo = _userRepository.GetUserByUsername(authDto.Username); 

            if (userFromRepo == null)
            {
                return Unauthorized("Kullanıcı bulunamadı");
            }

            if (!VerifyPasswordHash(authDto.Password, userFromRepo.PasswordHash, userFromRepo.PasswordSalt))
            {
                return Unauthorized("Şifre yanlış");
            }

            string token = CreateToken(userFromRepo);
            return Ok(new { token = token });
        } // <-- Login metodu burada BİTİYOR

        // JWT oluşturan metot
        private string CreateToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                //Bu jetonun sahibinin rolü user.role..name dir 
                //Bu,Authorization için kullanılacak en önemli bilgi 
                //UserRepository de .Include() kullandığımız için user.Role.Name artık dolu gelecek.
                new Claim(ClaimTypes.Role, user.Role.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Değişken adındaki boşluk kaldırıldı
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // "ClaimsIdentity" olarak düzeltildi
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // Şifre Hashleme Metotları
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}