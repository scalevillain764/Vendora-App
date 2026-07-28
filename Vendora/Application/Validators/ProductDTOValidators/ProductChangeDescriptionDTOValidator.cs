using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeDescriptionDTOValidator : AbstractValidator<ProductChangeDescriptionDTO>
    {
        public ProductChangeDescriptionDTOValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Максимальная длина описания 2000 символов.");
        }
    }
}