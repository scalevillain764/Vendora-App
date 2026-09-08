using Application.DTO.ProductDTO.CartDTO;
using Domain.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ICartService = Application.Interfaces.ICartService;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCartAsync(CancellationToken token)
            => ProcessResult(await _cartService.GetMyCartAsync(CurrentUserId, token));

        [HttpPost]
        [Route("products/{productId}")]
        public async Task<IActionResult> AddProductToCartAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _cartService.AddProductToCartAsync(CurrentUserId, productId, token));

        [HttpPatch]
        [Route("{productId}/increase")]
        public async Task<IActionResult> IncreaseQuantityAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _cartService.IncreaseQuantityAsync(CurrentUserId, productId, token));

        [HttpPatch]
        [Route("{productId}/decrease")]
        public async Task<IActionResult> DecreaseQuantityAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _cartService.DecreaseQuantityAsync(CurrentUserId, productId, token));

        [HttpDelete]
        [Route("{productId}/remove")]
        public async Task<IActionResult> RemoveProductFromCartAsync([FromRoute] Ulid productId)
           => ProcessResult(await _cartService.RemoveProductFromCartAsync(CurrentUserId, productId)); 
    }
}