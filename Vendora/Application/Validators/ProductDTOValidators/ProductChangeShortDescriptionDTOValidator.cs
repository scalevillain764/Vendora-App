using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeShortDescriptionDTOValidator : AbstractValidator<ProductChangeShortDescriptionDTO>
    {
        public ProductChangeShortDescriptionDTOValidator()
        {
            RuleFor(x => x.shortDescription)
                .MaximumLength(250).WithMessage("Максимальная длина краткого описания 250 символов.");
        }
    }
}