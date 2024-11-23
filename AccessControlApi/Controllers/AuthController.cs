using AccessControlApi.Domain.ApplicationInterfaces;
using AccessControlApi.Domain.Entities;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

namespace AccessControlApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        public AuthController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        // Login: recibir las credenciales y devolver un JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result.IsSuccessful)
                return Ok(result.Token);

            return Unauthorized(result.Message);
        }



        // Validar el token o refrescar token (opcional)
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshToken(request);
            if (result.IsSuccessful)
                return Ok(result.Token);

            return Unauthorized(result.Message);
        }
    }
}
