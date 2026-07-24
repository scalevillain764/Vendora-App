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

        [HttpPost]
        public async Task<IActionResult> CreatePendingOrderAsync()
            => ProcessResult(await _orderService.CreatePendingOrderAsync(CurrentUserId));

        [HttpPost]
        [Route("success/{OrderId}")]
        public async Task<IActionResult> SuccessOrderAsync(Ulid OrderId)
            => ProcessResult(await _orderService.ChangeOrderStatusToSuccessAsync(OrderId));

        [HttpPost]
        [Route("fail/{OrderId}")]
        public async Task<IActionResult> FailOrderAsync(Ulid OrderId)
            => ProcessResult(await _orderService.ChangeOrderStatusToFailAsync(OrderId));
    }
}