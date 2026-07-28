using Application.DTO.UserDTO;
using FluentValidation;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangeEmailDTOValidator: AbstractValidator<UserChangeEmailDTO>
    {
        public UserChangeEmailDTOValidator()
        {
            RuleFor(x => x.Email)
                .Matches(@"^\w+@\w+\.com$").WithMessage("Введите корректный email");
        }
    }
}