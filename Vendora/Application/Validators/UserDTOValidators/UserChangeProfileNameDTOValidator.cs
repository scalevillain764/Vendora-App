using Application.DTO.UserDTO;
using FluentValidation;
using System.Data;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangeProfileNameDTOValidator : AbstractValidator<UserChangeProfileNameDTO>
    {
        public UserChangeProfileNameDTOValidator()
        {
            RuleFor(x => x.ProfileName)
                .NotEmpty().WithMessage("Имя профиля не может быть пустым")
                .MaximumLength(25).WithMessage("Максимальная длина имени профиля 25 символов");
        }

    }
}