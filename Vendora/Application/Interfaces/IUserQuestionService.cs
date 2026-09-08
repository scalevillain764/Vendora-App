using Application.DTO.UserQuestionDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IUserQuestionService
    {
        Task<Result<UserQuestionResponseDTO>> AskQuestionAsync(Ulid UserId, Ulid ProductId, UserQuestionCreateAndChangeDTO DTO, CancellationToken token);
        Task<Result<UserQuestionResponseDTO>> DeleteQuestionAsync(Ulid UserId, Ulid QuestionId, CancellationToken token);
        Task<Result<UserQuestionResponseDTO>> EditQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionCreateAndChangeDTO DTO, CancellationToken token);
        Task<Result<UserQuestionResponseDTO>> ReplyUserQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionReplyDTO DTO, CancellationToken token);
        Task<Result<List<UserQuestionResponseDTO>>> GetUserQuestionsToProductAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
    }
}