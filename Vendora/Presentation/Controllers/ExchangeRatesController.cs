using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using IExchangeRatesService = Application.Interfaces.IExchangeRateService;
namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ExchangeRatesController : BaseController
    {
        private readonly IExchangeRatesService _service;
        public ExchangeRatesController(IExchangeRatesService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRatesAsync() => ProcessResult(await _service.GetExchangeRatesAsync());
    }
}