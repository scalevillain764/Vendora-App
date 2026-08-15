using IOrderService = Application.Interfaces.IOrderService;
using Application.DTO.OrderDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrdersAsync()
            => ProcessResult(await _orderService.GetMyOrdersAsync(CurrentUserId));

        [HttpPost]
        public async Task<IActionResult> CreatePendingOrderAsync()
            => ProcessResult(await _orderService.CreatePendingOrderAsync(CurrentUserId));
    }
}