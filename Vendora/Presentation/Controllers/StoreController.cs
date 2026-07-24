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

        [HttpGet]
        public async Task<IActionResult> GetMyStoreAsync()
            => ProcessResult(await _storeService.GetMyStoreAsync(CurrentUserId));

        [HttpGet]
        [Route("{StoreId}")]
        public async Task<IActionResult> GetStoreAsync(Ulid StoreId)
            => ProcessResult(await _storeService.GetStoreAsync(StoreId));

        [HttpPatch]
        [Route("name")]
        public async Task<IActionResult> ChangeStoreNameAsync(StoreChangeNameDTO DTO)
            => ProcessResult(await _storeService.ChangeStoreNameAsync(CurrentUserId, DTO));

        [HttpPatch]
        [Route("description")]
        public async Task<IActionResult> ChangeStoreDescriptionAsync(StoreChangeDescriptionDTO DTO)
            => ProcessResult(await _storeService.ChangeStoreDescriptionAsync(CurrentUserId, DTO));
    }
}