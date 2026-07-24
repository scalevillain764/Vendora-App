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
        public async Task<IActionResult> GetUserFavouritesAsync()
            => ProcessResult(await _service.GetFavouritesByIdAsync(CurrentUserId));

        [HttpPost]
        [Route("{ProductId}")]
        public async Task<IActionResult> AddFavouriteAsync(Ulid ProductId) 
            => ProcessResult(await _service.AddToFavouriteAsync(CurrentUserId, ProductId));


        [HttpDelete]
        [Route("{ProductId}")]
        public async Task<IActionResult> RemoveFromFavouriteAsync(Ulid ProductId)
            => ProcessResult(await _service.RemoveFromFavouriteAsync(CurrentUserId, ProductId));
    }
}