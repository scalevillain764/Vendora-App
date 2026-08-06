using Domain.UserQuestions;
namespace Application.DTO.UserQuestionDTO
{
    public record UserQuestionResponseDTO(Ulid Id, string? UserAvatarPicture, string UserProfileName,
        List<string>? Photos, string QuestionText, string? SellerReply, bool CanSellerReply,
        DateTime CreateAt)
    {
        public UserQuestionResponseDTO(UserQuestion question, bool CanSellerReply)
            : this(question.Id, question.user.AvatarUrl, question.user.ProfileName,
                  question.PhotoUrls, question.QuestionText, question.SellerReply, 
                  CanSellerReply, question.CreatedAt)
        { }
    }
}