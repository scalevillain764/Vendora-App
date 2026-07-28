using Application.DTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.StoreDTOValidators
{
    public class StoreOwnerCreateDTOValidator : AbstractValidator<StoreOwnerCreateDTO>
    {
        public StoreOwnerCreateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название магазина не может быть пустым.")
                .Length(15, 100).WithMessage("Длина названия магазина от 15 до 100 символов.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Длина описания магазина 2000 символов.");
        }
    }
}