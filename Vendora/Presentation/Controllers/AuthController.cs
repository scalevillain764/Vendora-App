using Application.DTO.AuthDTO;
using Application.DTO.UserDTO;
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
        public async Task<IActionResult> ResgistrateAsync([FromBody] UserRegistrationDTO DTO, CancellationToken token)
            => ProcessResult(await _authService.RegistrAsync(DTO, token));

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogInAsync([FromBody] UserLogInDTO DTO, CancellationToken token)
            => ProcessResult(await _authService.LogInAsync(DTO, token));

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync(CancellationToken token)
            => ProcessResult(await _authService.RefreshAsync(CurrentUserId, token));

        [HttpPatch]
        [Route("password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDTO DTO, CancellationToken token)
            => ProcessResult(await _authService.ChangeUserPasswordAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("login")]
        public async Task<IActionResult> ChangeLoginAsync([FromBody] UserChangeLoginDTO DTO, CancellationToken token)
            => ProcessResult(await _authService.ChangeUserLoginAsync(CurrentUserId, DTO, token));
    }
}