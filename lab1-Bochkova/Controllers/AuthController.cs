using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace lab1_bochkova.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Проверка правильности учетных данных пользователя
            if (model.Username == "admin" && model.Password == "admin")
            {
                // Определение секретного ключа для создания токена
                var key = Encoding.UTF8.GetBytes("ThisIsASecretKeyForJWT1234567890123456");
                var tokenHandler = new JwtSecurityTokenHandler();

                // Описание параметров токена (аудитория и эмитент должны совпадать с конфигурацией в Program.cs)
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = "BarbershopAPI", // Совпадает с Issuer в JwtBearerOptions
                    Audience = "BarbershopAPI", // Совпадает с Audience в JwtBearerOptions
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                // Создание токена
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Возврат токена клиенту
                return Ok(new { Token = tokenHandler.WriteToken(token) });
            }
            return Unauthorized();
        }
    }

    // Модель для получения данных логина
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
