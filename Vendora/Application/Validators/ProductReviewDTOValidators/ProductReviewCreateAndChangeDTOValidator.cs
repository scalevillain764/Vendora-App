using FluentValidation;
using Application.DTO.ProductReviewDTO;
namespace Application.Validators.ProductReviewValidators
{
    public class ProductReviewCreateAndChangeDTOValidator : AbstractValidator<ProductReviewCreationAndChangeDTO>
    {
        public ProductReviewCreateAndChangeDTOValidator()
        {
            RuleFor(x => x.ReviewText)
                .MaximumLength(1000).WithMessage("Длина отзыва должна быть не более 1000 символов");

            RuleFor(x => x.Rating)
                .GreaterThanOrEqualTo(0).WithMessage("Рейтинг должен быть положительным")
                .LessThanOrEqualTo(5).WithMessage("Рейтинг не должен превышать 5");
        }
    }
}