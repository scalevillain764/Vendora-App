using Microsoft.AspNetCore.Mvc;
using Application.DTO.ProductDTO.StatisticsDTO;
using IProductStatisticsService = Application.Interfaces.IProductStatisticsService;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ProductStatisticsController : BaseController
    {
        private readonly IProductStatisticsService _service;
        public ProductStatisticsController(IProductStatisticsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("{storeId}/{productId}")]
        public async Task<IActionResult> GetStatistics([FromRoute] Ulid storeId, [FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _service.GetProductStatisticsAsync(CurrentUserId, storeId, productId, token));
    }
}