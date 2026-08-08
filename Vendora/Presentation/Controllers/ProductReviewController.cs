using IProductReviewService = Application.Interfaces.IProductReviewService;
using Application.DTO.ProductReviewDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProductReviewController : BaseController {

        private readonly IProductReviewService _service;
        public ProductReviewController(IProductReviewService service)
            => _service = service;

        [HttpGet]
        [Route("{ProductId}")]
        public async Task<IActionResult> GetProductReviewByProductAsync(Ulid ProductId)
            => ProcessResult(await _service.GetProductReviewsAsync(CurrentUserId, ProductId));

        [HttpPost]
        [Route("{ProductId}")]
        public async Task<IActionResult> AddProductReviewAsync(Ulid ProductId, [FromBody] ProductReviewCreationAndChangeDTO DTO)
            => ProcessResult(await _service.AddProductReviewAsync(CurrentUserId, ProductId, DTO));

        [HttpDelete]
        [Route("{ReviewId}")]
        public async Task<IActionResult> RemoveProductReviewAsync(Ulid ReviewId)
            => ProcessResult(await _service.DeleteProductReviewAsync(CurrentUserId, ReviewId));

        [HttpPut]
        [Route("{ReviewId}")]
        public async Task<IActionResult> EditProductReviewAsync(Ulid ReviewId, ProductReviewCreationAndChangeDTO DTO)
            => ProcessResult(await _service.EditProductReviewAsync(CurrentUserId, ReviewId, DTO));

        [HttpPatch]
        [Route("{ReviewId}")]
        public async Task<IActionResult> ReplyToProductReviewAsync(Ulid ReviewId, ProductReviewSellerReplyDTO DTO)
            => ProcessResult(await _service.ReplyProductReviewAsync(CurrentUserId, ReviewId, DTO));
    }
}