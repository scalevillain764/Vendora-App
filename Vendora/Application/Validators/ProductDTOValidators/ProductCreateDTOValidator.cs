using Application.DTO.ProductDTO.StoreDTO;
using FluentValidation;
namespace Application.Validators.ProductDTOValidators
{
    public class ProductCreationDTOValidator : AbstractValidator<ProductCreationDTO>
    {
        public ProductCreationDTOValidator()
        {
            RuleFor(x => x.Name)
              .NotEmpty().WithMessage("Название продукта не может быть пустым.")
              .Length(2, 100).WithMessage("Длина названия от 2 до 100 символов.");

            RuleFor(x => x.StoreId)
                .NotEmpty().WithMessage("Сначала создайте магазин");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Указана несуществующая категория товара.");

            RuleFor(x => x.Description)
               .MaximumLength(2000).WithMessage("Максимальная длина описания 2000 символов.");

            RuleFor(x => x.ShortDescription)
               .MaximumLength(250).WithMessage("Максимальная длина краткого описания 250 символов.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена не может быть меньше 0");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Количество не может быть меньше 0");
        }
    }
}