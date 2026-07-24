using Application.DTO.AuthDTO;
using Microsoft.AspNetCore.Mvc;
using IAuthService = Application.Interfaces.IAuthService;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> ResgistrateAsync([FromBody] UserRegistrationDTO DTO)
        {
            var rez = await _authService.RegistrAsync(DTO);
            return ProcessResult(rez);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogInAsync([FromBody] UserLogInDTO DTO)
        {
            var rez = await _authService.LogInAsync(DTO);
            return ProcessResult(rez);
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync()
        {
            var userId = GetUserId();
            var rez = await _authService.RefreshAsync(userId);
            return ProcessResult(rez);
        }
    }
}