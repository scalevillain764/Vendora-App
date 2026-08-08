using Application.DTO.UserQuestionDTO;
using FluentValidation;
namespace Application.Validators.UserQuestionDTOValidators
{
    public class UserQuestionCreateAndChangeDTOValidator : AbstractValidator<UserQuestionCreateAndChangeDTO>
    {
        public UserQuestionCreateAndChangeDTOValidator()
        {
            RuleFor(x => x.QuestionText)
                .MinimumLength(2000).WithMessage("Длина вопроса должна быть не более 2000 символов");
        }
    }
}