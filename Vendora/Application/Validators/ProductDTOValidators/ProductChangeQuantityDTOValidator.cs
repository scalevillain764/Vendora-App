using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeQuantityDTOValidator : AbstractValidator<ProductChangeQuantityDTO>
    {
        public ProductChangeQuantityDTOValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Количество не может быть меньше 0");
        }
    }
}