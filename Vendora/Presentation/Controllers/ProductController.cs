using Microsoft.AspNetCore.Mvc;
using IProductService = Application.Interfaces.IProductService;
using Application.DTO.ProductDTO.StoreDTO;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> CreateProductAsync(ProductCreationDTO DTO)
            => ProcessResult(await _productService.CreateProductAsync(CurrentUserId, DTO));

        [HttpDelete]
        [Route("{ProductId}")]
        public async Task<IActionResult> RemoveProductAsync(Ulid ProductId) 
            => ProcessResult(await _productService.RemoveProductAsync(CurrentUserId, ProductId));

        [HttpPatch]
        [Route("name/{ProductId}")]
        public async Task<IActionResult> ChangeProductNameAsync(Ulid ProductId, ProductChangeNameDTO DTO)
            => ProcessResult(await _productService.ChangeProductNameAsync(CurrentUserId, ProductId, DTO));

        [HttpPatch]
        [Route("category/{ProductId}")]
        public async Task<IActionResult> ChangeProductCategoryAsync(Ulid ProductId, ProductChangeCategoryDTO DTO)
            => ProcessResult(await _productService.ChangeProductCategoryAsync(CurrentUserId, ProductId, DTO));

        [HttpPatch]
        [Route("quantity/{ProductId}")]
        public async Task<IActionResult> ChangeProductQuantityAsync(Ulid ProductId, ProductChangeQuantityDTO DTO)
            => ProcessResult(await _productService.ChangeProductQuantityAsync(CurrentUserId, ProductId, DTO));

        [HttpPatch]
        [Route("description/{ProductId}")]
        public async Task<IActionResult> ChangeProductDescriptionAsync(Ulid ProductId, ProductChangeDescriptionDTO DTO)
            => ProcessResult(await _productService.ChangeProductDescriptionAsync(CurrentUserId, ProductId, DTO));

        [HttpPatch]
        [Route("price/{ProductId}")]
        public async Task<IActionResult> ChangeProductPriceAsync(Ulid ProductId, ProductChangePriceDTO DTO)
           => ProcessResult(await _productService.ChangeProductPriceAsync(CurrentUserId, ProductId, DTO));

        [HttpPatch]
        [Route("short_description/{ProductId}")]
        public async Task<IActionResult> ChangeProductShortDescriptionAsync(Ulid ProductId, ProductChangeShortDescriptionDTO DTO)
          => ProcessResult(await _productService.ChangeProductShortDescriptionAsync(CurrentUserId, ProductId, DTO));

        [HttpGet]
        [Route("{ProductId}")]
        public async Task<IActionResult> GetProductByIdAsync(Ulid ProductId)
            => ProcessResult(await _productService.GetProduct(ProductId));
    }
}