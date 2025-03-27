using Microsoft.AspNetCore.Mvc;
using UserServiceRepository.Interface;
using ObjectTrackerBackend.Models;
using TokenServiceRepository.Interface;
using System.ComponentModel.DataAnnotations;

namespace ObjectTrackerBackend.Controllers
{    
    [ApiController]
    [Route("/api/auth")]
    public class AuthorizationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public AuthorizationController(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.AuthenticateAsync(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Невірний логін або пароль");

            var token = Guid.NewGuid().ToString();
            await _tokenRepository.StoreTokenAsync(token, user.Id, TimeSpan.FromMinutes(15));

            return Ok(new { token });
        }

        [HttpGet("isTokenValid/{token}", Name = nameof(isTokenValid))]
        public async Task<IActionResult> isTokenValid([Required] string token)
        {
            var userId = await _tokenRepository.GetUserIdByTokenAsync(token);
            if (userId == null)
                return Unauthorized();
            return Ok();
        }
    }
}
