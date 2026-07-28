using Application.DTO.UserDTO;
using FluentValidation;
using System.Data;
using Domain.Users;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangeLastNameDTOValidator : AbstractValidator<UserChangeLastNameDTO>
    {
        public UserChangeLastNameDTOValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия не может быть пустой");
        }

    }
}