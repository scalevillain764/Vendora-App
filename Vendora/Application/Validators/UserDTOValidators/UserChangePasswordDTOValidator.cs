using Application.DTO.UserDTO;
using FluentValidation;
namespace Application.Validators.UserLogInDTOValidators
{
    public class UserChangePasswordDTOValidator : AbstractValidator<UserChangePasswordDTO>
    {
        public UserChangePasswordDTOValidator()
        {           
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Пароль не должен быть пустым")
                .Length(8, 12).WithMessage("Длина пароля не должна быть от 8 до 12 символов.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])")
                .WithMessage("Пароль должен содержать заглавную и строчную буквы, цифру и спецсимвол.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Пароль не должен быть пустым")
                .Length(8, 12).WithMessage("Длина пароля не должна быть от 8 до 12 символов.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])")
                .WithMessage("Пароль должен содержать заглавную и строчную буквы, цифру и спецсимвол.");
        }
    }
}