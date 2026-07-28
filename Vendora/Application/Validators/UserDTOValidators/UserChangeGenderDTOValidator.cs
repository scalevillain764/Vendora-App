using Application.DTO.UserDTO;
using FluentValidation;
using System.Data;
using Domain.Users;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangeGenderDTOValidator : AbstractValidator<UserChangeGenderDTO>
    {
        public UserChangeGenderDTOValidator()
        {
            RuleFor(x => x.Gender)
                .Must(id => Enum.IsDefined(typeof(User.Gender), id))
                .WithMessage("Указана несуществующая категория товара.");
        }

    }
}