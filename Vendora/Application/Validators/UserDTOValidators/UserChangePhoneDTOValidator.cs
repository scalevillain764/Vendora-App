using Application.DTO.UserDTO;
using FluentValidation;
using System.Data;
using Domain.Users;
namespace Application.Validators.UserDTOValidators
{
    public class UserChangePhoneDTOValidator: AbstractValidator<UserChangePhoneDTO>
    {
        public UserChangePhoneDTOValidator()
        {
            RuleFor(x => x.Phone)
                .Length(13)
                .Matches(@"^\+375\d{9}$").WithMessage("Рроверьте указанный номер.");
        }
    }
}