using Application.DTO.SearchDTO;
using FluentValidation;
namespace Application.Validators.SearchDTOValidator
{
    public class SearchDTOValidator: AbstractValidator<SearchRequestDTO>
    {
        public SearchDTOValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThan(0).WithMessage("Минимальная цена должна быть больше нуля");

            RuleFor(x => x.MaxPrice)
                .LessThan(1000000000).WithMessage("Максимальная цена должна быть меньше 1000000000");
        }
    }
}