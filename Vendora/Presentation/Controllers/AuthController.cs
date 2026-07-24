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
            => ProcessResult(await _authService.RegistrAsync(DTO));

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogInAsync([FromBody] UserLogInDTO DTO)
            => ProcessResult(await _authService.LogInAsync(DTO));

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync()
            => ProcessResult(await _authService.RefreshAsync(CurrentUserId));
    }
}