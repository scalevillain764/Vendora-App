using Application.DTO.AuthDTO;
using FluentValidation;
namespace Application.Validators.UserRegistrationDTOValidators
{
    public class UserLogInValidator : AbstractValidator<UserRegistrationDTO>
    {
        public UserLogInValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Логин не может быть пустым")
                .Length(3, 20).WithMessage("Длина логина должна быть от 3 до 20 символов.")
                .Matches(@"^[a-zA-Z0-9_-]+$")
                .WithMessage("Логин может содержать только английские буквы, цифры, знаки '-' и '_'.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль не должен быть пустым")
                .Length(8, 12).WithMessage("Длина пароля не должна быть от 8 до 12 символов.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])")
                .WithMessage("Пароль должен содержать заглавную и строчную буквы, цифру и спецсимвол.");
        }
    }
}