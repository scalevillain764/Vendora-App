using Application.DTO.UserQuestionDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IUserQuestionService
    {
        Task<Result<UserQuestionResponseDTO>> AskQuestionAsync(Ulid UserId, Ulid ProductId, UserQuestionCreateAndChangeDTO DTO);
        Task<Result<UserQuestionResponseDTO>> RemoveQuestionAsync(Ulid UserId, Ulid QuestionId);
        Task<Result<UserQuestionResponseDTO>> ChangeQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuestionCreateAndChangeDTO DTO);
        Task<Result<UserQuestionResponseDTO>> ReplyUserQuestionAsync(Ulid UserId, Ulid QuestionId, UserQuastionReplyDTO DTO);
    }
}