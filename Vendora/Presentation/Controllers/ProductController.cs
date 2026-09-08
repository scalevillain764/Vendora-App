using Microsoft.AspNetCore.Mvc;
using IProductService = Application.Interfaces.IProductService;
using Application.DTO.ProductDTO.StoreDTO;
using Microsoft.AspNetCore.Authorization;
using Application.DTO.ProductDTO;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController: BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductCreationDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.CreateProductAsync(CurrentUserId, DTO, token));

        [HttpDelete]
        [Route("{productId}")]
        public async Task<IActionResult> RemoveProductAsync([FromRoute] Ulid productId, CancellationToken token) 
            => ProcessResult(await _productService.RemoveProductAsync(CurrentUserId, productId, token));

        [HttpPatch]
        [Route("{productId}/name")]
        public async Task<IActionResult> ChangeProductNameAsync([FromRoute] Ulid productId, [FromBody] ProductChangeNameDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.ChangeProductNameAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/category")]
        public async Task<IActionResult> ChangeProductCategoryAsync([FromRoute] Ulid productId, [FromBody] ProductChangeCategoryDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.ChangeProductCategoryAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/quantity")]
        public async Task<IActionResult> ChangeProductQuantityAsync([FromRoute] Ulid productId, [FromBody] ProductChangeQuantityDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.ChangeProductQuantityAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/description")]
        public async Task<IActionResult> ChangeProductDescriptionAsync([FromRoute] Ulid productId, [FromRoute] ProductChangeDescriptionDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.ChangeProductDescriptionAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/price")]
        public async Task<IActionResult> ChangeProductPriceAsync([FromRoute] Ulid productId, [FromBody] ProductChangePriceDTO DTO, CancellationToken token)
           => ProcessResult(await _productService.ChangeProductPriceAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/short_description")]
        public async Task<IActionResult> ChangeProductShortDescriptionAsync([FromRoute] Ulid productId, [FromBody] ProductChangeShortDescriptionDTO DTO, CancellationToken token)
          => ProcessResult(await _productService.ChangeProductShortDescriptionAsync(CurrentUserId, productId, DTO, token));

        [HttpGet]
        [Route("store/{storeId}")]
        public async Task<IActionResult> GetProductsFromStoreAsync([FromRoute] Ulid storeId, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken token)
            => ProcessResult(await _productService.GetProductsFromStoreAsync(CurrentUserId, new ProductsGetFromStoreDTO(storeId, page, pageSize), token));

        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _productService.GetProductAsync(CurrentUserId, productId, token));

        [HttpPatch]
        [Route("{productId}/preview")]
        public async Task<IActionResult> ChangeProductPreviewPictureAsync([FromRoute] Ulid productId, [FromForm] IFormFile? file, CancellationToken token)
            => ProcessResult(await _productService.ChangeProductPreviewPictureAsync(CurrentUserId, productId, file, token));      

        [HttpPatch]
        [Route("{productId}/images/add")]
        public async Task<IActionResult> AddImagesToProductAsync([FromRoute] Ulid productId, [FromBody] ProductAddPicturesDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.AddPicturesToProductAsync(CurrentUserId, productId, DTO, token));

        [HttpPatch]
        [Route("{productId}/images/remove")]
        public async Task<IActionResult> RemoveImageFromProductAsync([FromRoute] Ulid productId, ProductRemovePictureDTO DTO, CancellationToken token)
            => ProcessResult(await _productService.RemovePictureFromProduct(CurrentUserId, productId, DTO, token));
    }
}