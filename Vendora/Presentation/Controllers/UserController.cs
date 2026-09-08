using Application.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IUserService = Application.Interfaces.IUserService;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMyAccountAsync(CancellationToken token)
            => ProcessResult(await _userService.DeleteMyAccountAsync(CurrentUserId, token));

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetMeAsync(CancellationToken token)
            => ProcessResult(await _userService.GetMeAsync(CurrentUserId, token));

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] Ulid userId, CancellationToken token)
            => ProcessResult(await _userService.GetUserAsync(userId, token));

        [HttpPatch]
        [Route("profile_name")]
        public async Task<IActionResult> ChangeUserProfileNameAsync([FromBody] UserChangeProfileNameDTO DTO, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserProfileNameAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("first_name")]
        public async Task<IActionResult> ChangeUserFirstNameAsync([FromBody] UserChangeFirstNameDTO DTO, CancellationToken token)
           => ProcessResult(await _userService.ChangeUserFirstNameAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("last_name")]
        public async Task<IActionResult> ChangeUserLastNameAsync([FromBody] UserChangeLastNameDTO DTO, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserLastNameAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("email")]
        public async Task<IActionResult> ChangeUserEmailAsync([FromBody] UserChangeEmailDTO DTO, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserEmailAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("phone")]
        public async Task<IActionResult> ChangeUserPhoneAsync([FromBody] UserChangePhoneDTO DTO, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserPhoneAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("gender")]
        public async Task<IActionResult> ChangeUserGenderAsync([FromBody] UserChangeGenderDTO DTO, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserGenderAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("profile_picture")]
        public async Task<IActionResult> ChangeUserProfilePictureAsync([FromForm] IFormFile? file, CancellationToken token)
            => ProcessResult(await _userService.ChangeUserProfilePictureAsync(CurrentUserId, file, token));
    }
}