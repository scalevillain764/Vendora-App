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
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmYooKassaWebhook([FromBody] PaymentYooKassaRequestDTO DTO, CancellationToken token)
            => ProcessResult(await _paymentService.ConfirmYooKassaPaymentAsync(CurrentUserId, DTO, token));

        [HttpPost]
        [Route("{orderId}/balance")]
        public async Task<IActionResult> PayFromBalance(Ulid orderId, CancellationToken token)
            => ProcessResult(await _paymentService.PayFromBalanceAsync(CurrentUserId, orderId, token));

        [HttpPost]
        [Route("{orderId}/yookassa/init")]
        public async Task<IActionResult> PayFromYookassa([FromRoute] Ulid orderId, CancellationToken token)
            => ProcessResult(await _paymentService.PayFromYOOKassaAsync(CurrentUserId, orderId, token));
    }
}