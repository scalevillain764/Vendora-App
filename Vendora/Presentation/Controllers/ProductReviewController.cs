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
        [Route("{productId}")]
        public async Task<IActionResult> GetProductReviewByProductAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _service.GetProductReviewsAsync(CurrentUserId, productId, token));

        [HttpPost]
        [Route("{productId}")]
        public async Task<IActionResult> AddProductReviewAsync([FromRoute] Ulid productId, [FromBody] ProductReviewCreationAndChangeDTO DTO, CancellationToken token)
            => ProcessResult(await _service.AddProductReviewAsync(CurrentUserId, productId, DTO, token));

        [HttpDelete]
        [Route("{reviewId}")]
        public async Task<IActionResult> RemoveProductReviewAsync([FromRoute] Ulid reviewId, CancellationToken token)
            => ProcessResult(await _service.DeleteProductReviewAsync(CurrentUserId, reviewId, token));

        [HttpPut]
        [Route("{reviewId}")]
        public async Task<IActionResult> EditProductReviewAsync([FromRoute] Ulid reviewId, [FromBody] ProductReviewCreationAndChangeDTO DTO, CancellationToken token)
            => ProcessResult(await _service.EditProductReviewAsync(CurrentUserId, reviewId, DTO, token));

        [HttpPatch]
        [Route("{reviewId}")]
        public async Task<IActionResult> ReplyToProductReviewAsync([FromRoute] Ulid reviewId, [FromBody] ProductReviewSellerReplyDTO DTO, CancellationToken token)
            => ProcessResult(await _service.ReplyProductReviewAsync(CurrentUserId, reviewId, DTO, token));
    }
}