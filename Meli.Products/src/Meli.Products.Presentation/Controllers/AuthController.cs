using Microsoft.AspNetCore.Mvc;
using Meli.Products.Application.DTOs;
using Meli.Products.Infrastructure.Security;

namespace Meli.Products.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validación básica - reemplaza con tu lógica real
            if (request.Email == "test@test.com" && request.Password == "123456")
            {
                var token = _tokenService.GenerateToken("123", request.Email);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Expires = DateTime.UtcNow.AddHours(2)
                });
            }

            return Unauthorized("Credenciales inválidas");
        }
    }
}
