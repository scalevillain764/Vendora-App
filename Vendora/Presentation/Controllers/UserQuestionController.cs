using Application.DTO.UserQuestionDTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IUserQuestionService = Application.Interfaces.IUserQuestionService;
namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserQuestionController : BaseController {
        private readonly IUserQuestionService _service;
        public UserQuestionController(IUserQuestionService service)
            => _service = service;

        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> GetUserQuestionsToProductAsync([FromRoute] Ulid productId, CancellationToken token)
            => ProcessResult(await _service.GetUserQuestionsToProductAsync(CurrentUserId, productId, token));

        [HttpPost]
        [Route("{productId}")]
        public async Task<IActionResult> AskUserQuestionAsync([FromRoute] Ulid productId, [FromBody] UserQuestionCreateAndChangeDTO DTO, CancellationToken token)
            => ProcessResult(await _service.AskQuestionAsync(CurrentUserId, productId, DTO, token));

        [HttpDelete]
        [Route("{questionId}")]
        public async Task<IActionResult> DeleteUserQuestionAsync([FromRoute] Ulid questionId, CancellationToken token)
            => ProcessResult(await _service.DeleteQuestionAsync(CurrentUserId, questionId, token));

        [HttpPut]
        [Route("{questionId}")]
        public async Task<IActionResult> EditUserQuestionAsync([FromRoute] Ulid questionId, [FromBody] UserQuestionCreateAndChangeDTO DTO, CancellationToken token)
            => ProcessResult(await _service.EditQuestionAsync(CurrentUserId, questionId, DTO, token));

        [HttpPatch]
        [Route("{questionId}")]
        public async Task<IActionResult> ReplyToQuestionAsync([FromRoute] Ulid questionId, [FromBody] UserQuestionReplyDTO DTO, CancellationToken token)
            => ProcessResult(await _service.ReplyUserQuestionAsync(CurrentUserId, questionId, DTO, token));
    }
}