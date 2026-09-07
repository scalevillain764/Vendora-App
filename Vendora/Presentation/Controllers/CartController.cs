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
        public async Task<IActionResult> GetMyCartAsync()
            => ProcessResult(await _cartService.GetMyCartAsync(CurrentUserId));

        [HttpPost]
        [Route("products/{productId}")]
        public async Task<IActionResult> AddProductToCartAsync(Ulid productId)
            => ProcessResult(await _cartService.AddProductToCartAsync(CurrentUserId, productId));

        [HttpPatch]
        [Route("products/{cartItemId}/increase")]
        public async Task<IActionResult> IncreaseQuantityAsync(Ulid cartItemId)
            => ProcessResult(await _cartService.IncreaseQuantityAsync(CurrentUserId, cartItemId));

        [HttpPatch]
        [Route("products/{cartItemId}/decrease")]
        public async Task<IActionResult> DecreaseQuantityAsync(Ulid cartItemId)
            => ProcessResult(await _cartService.DecreaseQuantityAsync(CurrentUserId, cartItemId));

        [HttpDelete]
        [Route("products/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItemAsync(Ulid cartItemId)
           => ProcessResult(await _cartService.RemoveProductFromCartAsync(CurrentUserId, cartItemId)); 
    }
}