using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeNameDTOValidator : AbstractValidator<ProductChangeNameDTO>
    {
        public ProductChangeNameDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название продукта не может быть пустым.")
                .Length(2, 100).WithMessage("Длина названия от 2 до 100 символов.");
        }
    }
}