using Microsoft.AspNetCore.Mvc;
using Application.DTO.PaymentDTO;
using IPaymentService = Application.Interfaces.IPaymentService;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : BaseController { 
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("yookassa/webhook")]
        public async Task<IActionResult> ConfirmYooKassaWebhook(PaymentYooKassaRequestDTO DTO)
            => ProcessResult(await _paymentService.ConfirmYooKassaPaymentAsync(CurrentUserId, DTO));

        [HttpPost]
        [Route("balance/{OrderId}")]
        public async Task<IActionResult> PayFromBalance(Ulid OrderId)
            => ProcessResult(await _paymentService.PayFromBalanceAsync(CurrentUserId, OrderId));

        [HttpPost]
        [Route("yookassa/init/{OrderId}")]
        public async Task<IActionResult> PayFromYookassa(Ulid OrderId)
            => ProcessResult(await _paymentService.PayFromYOOKassaAsync(CurrentUserId, OrderId));
    }
}