using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangePriceDTOValidator : AbstractValidator<ProductChangePriceDTO>
    {
        public ProductChangePriceDTOValidator()
        {
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена не может быть меньше 0");
        }
    }
}