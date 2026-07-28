using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
using static Domain.Products.Product;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeCategoryDTOValidator : AbstractValidator<ProductChangeCategoryDTO>
    {
        public ProductChangeCategoryDTOValidator()
        {
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Указана несуществующая категория товара.");
        }
    }
}