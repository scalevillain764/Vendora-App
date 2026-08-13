using Application.DTO.SearchDTO;
using Application.Result;
using ISearchService = Application.Interfaces.ISearchService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : BaseController {
        private readonly ISearchService _service;
        public SearchController (ISearchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync([FromBody] SearchRequestDTO DTO)
            => ProcessResult(await _service.SearchAsync(CurrentUserId, DTO));
    }
}

