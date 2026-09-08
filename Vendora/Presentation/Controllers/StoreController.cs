using Microsoft.AspNetCore.Mvc;
using IStoreService = Application.Interfaces.IStoreService;
using Application.DTO.StoreDTO;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StoreController: BaseController
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMyStoreAsync(CancellationToken token)
            => ProcessResult(await _storeService.RemoveMyStoreAsync(CurrentUserId, token));

        [HttpPost]
        public async Task<IActionResult> CreateStoreAsync([FromBody] StoreOwnerCreateDTO DTO, CancellationToken token)
            => ProcessResult(await _storeService.CreateStoreAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("profile_picture")]
        public async Task<IActionResult> ChangeStoreProfilePictureAsync([FromForm] IFormFile? file, CancellationToken token)
            => ProcessResult(await _storeService.ChangeStoreAvatarAsync(CurrentUserId, file, token));

        [HttpGet]
        public async Task<IActionResult> GetMyStoreAsync(CancellationToken token)
            => ProcessResult(await _storeService.GetMyStoreAsync(CurrentUserId, token));

        [HttpGet]
        [Route("{storeId}")]
        public async Task<IActionResult> GetStoreAsync([FromRoute] Ulid storeId, CancellationToken token)
            => ProcessResult(await _storeService.GetStoreAsync(storeId, token));

        [HttpPatch]
        [Route("name")]
        public async Task<IActionResult> ChangeStoreNameAsync([FromBody] StoreChangeNameDTO DTO, CancellationToken token)
            => ProcessResult(await _storeService.ChangeStoreNameAsync(CurrentUserId, DTO, token));

        [HttpPatch]
        [Route("description")]
        public async Task<IActionResult> ChangeStoreDescriptionAsync([FromBody] StoreChangeDescriptionDTO DTO, CancellationToken token)
            => ProcessResult(await _storeService.ChangeStoreDescriptionAsync(CurrentUserId, DTO, token));
    }
}