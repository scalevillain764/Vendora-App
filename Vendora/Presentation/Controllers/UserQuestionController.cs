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
        [Route("{ProductId}")]
        public async Task<IActionResult> GetUserQuestionsToProductAsync(Ulid ProductId)
            => ProcessResult(await _service.GetUserQuestionsToProductAsync(CurrentUserId, ProductId));

        [HttpPost]
        [Route("{ProductId}")]
        public async Task<IActionResult> AskUserQuestionAsync(Ulid ProductId, [FromBody] UserQuestionCreateAndChangeDTO DTO)
            => ProcessResult(await _service.AskQuestionAsync(CurrentUserId, ProductId, DTO));

        [HttpDelete]
        [Route("{QuestionId}")]
        public async Task<IActionResult> DeleteUserQuestionAsync(Ulid QuestionId)
            => ProcessResult(await _service.DeleteQuestionAsync(CurrentUserId, QuestionId));

        [HttpPut]
        [Route("{QuestionId}")]
        public async Task<IActionResult> EditUserQuestionAsync(Ulid QuestionId, [FromBody] UserQuestionCreateAndChangeDTO DTO)
            => ProcessResult(await _service.EditQuestionAsync(CurrentUserId, QuestionId, DTO));

        [HttpPatch]
        [Route("{QuestionId}")]
        public async Task<IActionResult> ReplyToQuestionAsync(Ulid QuestionId, [FromBody] UserQuestionReplyDTO DTO)
            => ProcessResult(await _service.ReplyUserQuestionAsync(CurrentUserId, QuestionId, DTO));
    }
}