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
            // Gönderilen kullanıcı adının zaten alınıp alınmadığını kontrol et
            if (_userRepository.UserExists(authDto.Username)) 
            {
                return BadRequest("Bu kullanıcı adı zaten alınmış.");
            }

            // Güvenli şifreleme için hash ve salt oluştur.
            CreatePasswordHash(authDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Yeni kullanıcı nesnesi oluştur.
            var userToCreate = new UserModel
            {
                Username = authDto.Username, // "Username" olarak düzeltildi
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = 1 // Varsayılan rol
            };

            _userRepository.RegisterUser(userToCreate);
            _userRepository.Save();

            return Ok("Kullanıcı Başarıyla Oluşturuldu");
        } // <-- Register metodu burada BİTİYOR

        // --- GİRİŞ (LOGIN) ENDPOINT'İ (AYRI BİR METOT OLARAK) ---
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthDto authDto)
        {
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