namespace Application.DTO.UserQuestionDTO
{
    public record UserQuestionCreateAndChangeDTO(
        Ulid UserId,
        Ulid ProductId,
        Ulid StoreId,
        string? QuestionText,
        List<string>? PhotoUrl,
        DateTime CreatedAt
    );
}