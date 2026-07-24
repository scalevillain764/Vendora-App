using Microsoft.AspNetCore.Mvc;
using ICartService = Application.Interfaces.ICartService;
using Application.DTO.ProductDTO.CartDTO;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCartAsync()
        {
            var ULID_userId = GetUserId();
            var rez = await _cartService.GetMyCartAsync(ULID_userId);
            return ProcessResult(rez);
        }

        [HttpPost]
        [Route("items/{cartItemId}/increase")]
        public async Task<IActionResult> IncreaseQuantityAsync(Ulid cartItemId)
        {    
            var ULID_userId = GetUserId();
            var rez = await _cartService.IncreaseQuantityAsync(ULID_userId, cartItemId);
            return ProcessResult(rez);
        }

        [HttpPost]
        [Route("items/{cartItemId}/dencrease")]
        public async Task<IActionResult> DecreaseQuantityAsync(Ulid cartItemId)
        {
            var ULID_userId = GetUserId();
            var rez = await _cartService.DecreaseQuantityAsync(ULID_userId, cartItemId);
            return ProcessResult(rez);
        }

        [HttpDelete]
        [Route("items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItemAsync(Ulid cartItemId)
        {
            var ULID_userId = GetUserId();
            var rez = await _cartService.RemoveCartItemAsync(ULID_userId, cartItemId);
            return ProcessResult(rez);
        }
    }
}