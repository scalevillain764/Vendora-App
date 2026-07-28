using Application.DTO.UserDTO;
using FluentValidation;
using System.Data;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangeFirstNameDTOValidator: AbstractValidator<UserChangeFirstNameDTO>
    {
        public UserChangeFirstNameDTOValidator() {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя не может быть пустое.")
                .MinimumLength(3).WithMessage("Минимальная длина имени 3 символа");
        }
        
    }
}