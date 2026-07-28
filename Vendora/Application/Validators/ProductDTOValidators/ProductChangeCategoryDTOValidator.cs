using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
using static Domain.Orders.Order;
using static Domain.Products.Product;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductChangeCategoryDTOValidator : AbstractValidator<ProductChangeCategoryDTO>
    {
        public ProductChangeCategoryDTOValidator()
        {
            RuleFor(x => x.Category)
               .Must(id => Enum.IsDefined(typeof(ProductCategory), id)).
                WithMessage("Указана несуществующая категория товара.");
        }
    }
}