using Application.DTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.StoreDTOValidators
{
    public class StoreChangeDescriptionDTOValidator: AbstractValidator<StoreChangeDescriptionDTO>
    {
        public StoreChangeDescriptionDTOValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Длина описания магазина 2000 символов.");
        }
    }
}