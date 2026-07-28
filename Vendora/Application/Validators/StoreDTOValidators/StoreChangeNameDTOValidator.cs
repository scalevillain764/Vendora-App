using Application.DTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.StoreDTOValidators
{
    public class StoreChangeNameDTOValidator : AbstractValidator<StoreChangeNameDTO>
    {
        public StoreChangeNameDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название магазина не может быть пустым.")
                .Length(15, 100).WithMessage("Длина названия магазина от 15 до 100 символов.");
        }
    }
}