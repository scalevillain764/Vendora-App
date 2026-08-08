using Application.DTO.UserQuestionDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IUserQuestionService
    {
        Task<Result<UserQuestionResponseDTO>> AskQuestionAsync(Ulid UserId, Ulid ProductId, UserQuestionCreateAndChangeDTO DTO);
        Task<Result<UserQuestionResponseDTO>> DeleteQuestionAsync(Ulid UserId, Ulid QuestionId);
        Task<Result<UserQuestionResponseDTO>> EditQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionCreateAndChangeDTO DTO);
        Task<Result<UserQuestionResponseDTO>> ReplyUserQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionReplyDTO DTO);
        Task<Result<List<UserQuestionResponseDTO>>> GetUserQuestionsToProductAsync(Ulid UserId, Ulid ProductId);
    }
}