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

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetMeAsync()
            => ProcessResult(await _userService.GetMeAsync(CurrentUserId));

        [HttpDelete]
        public async Task<IActionResult> DeleteMyAccountAsync()
            => ProcessResult(await _userService.DeleteYourMyAsync(CurrentUserId));

        [HttpGet]
        [Route("{UserId}")]
        public async Task<IActionResult> GetUserAsync(Ulid UserId)
            => ProcessResult(await _userService.GetUserAsync(UserId));

        [HttpPatch]
        [Route("profile_name")]
        public async Task<IActionResult> ChangeUserProfileNameAsync(UserChangeProfileNameDTO DTO)
            => ProcessResult(await _userService.ChangeUserProfileNameAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("first_name")]
        public async Task<IActionResult> ChangeUserFirstNameAsync(UserChangeFirstNameDTO DTO)
           => ProcessResult(await _userService.ChangeUserFirstNameAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("last_name")]
        public async Task<IActionResult> ChangeUserLastNameAsync(UserChangeLastNameDTO DTO)
            => ProcessResult(await _userService.ChangeUserLastNameAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("email")]
        public async Task<IActionResult> ChangeUserEmailAsync(UserChangeEmailDTO DTO)
            => ProcessResult(await _userService.ChangeUserEmailAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("phone")]
        public async Task<IActionResult> ChangeUserPhoneAsync(UserChangePhoneDTO DTO)
            => ProcessResult(await _userService.ChangeUserPhoneAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("gender")]
        public async Task<IActionResult> ChangeUserGenderAsync(UserChangeGenderDTO DTO)
            => ProcessResult(await _userService.ChangeUserGenderAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("profilePicture")]
        public async Task<IActionResult> ChangeUserProfilePictureAsync(IFormFile file)
            => ProcessResult(await _userService.ChangeUserProfilePictureAsync(CurrentUserId, file));
    }
}