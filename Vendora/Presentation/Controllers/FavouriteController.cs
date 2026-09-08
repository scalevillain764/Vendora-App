using Microsoft.AspNetCore.Mvc;
using IFavouriteService = Application.Interfaces.IFavouriteService;
using Application.DTO.FavouriteDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavouriteController : BaseController
    {
        private readonly IFavouriteService _service;
        public FavouriteController(IFavouriteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFavouritesAsync(CancellationToken token)
            => ProcessResult(await _service.GetFavouritesByIdAsync(CurrentUserId, token));

        [HttpPost]
        [Route("{productId}")]
        public async Task<IActionResult> AddFavouriteAsync([FromRoute] Ulid productId, CancellationToken token) 
            => ProcessResult(await _service.AddToFavouriteAsync(CurrentUserId, productId, token));


        [HttpDelete]
        [Route("{productId}")]
        public async Task<IActionResult> RemoveFromFavouriteAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _service.RemoveFromFavouriteAsync(CurrentUserId, productId, token));
    }
}