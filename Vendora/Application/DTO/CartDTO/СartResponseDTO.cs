using Application.DTO.ProductDTO.CartDTO;
using Domain.Carts;
namespace Application.DTO.CartDTO
{
    public record CartResponseDTO(
        Ulid UserId,
        List<ProductCartCardResponseDTO> cartItems,
        int TotalQuantity,
        decimal TotalPrice
        )
    {
        public CartResponseDTO(Cart cart) :
            this(cart.UserId,
                cart.Items.Select(x => new ProductCartCardResponseDTO(x)).ToList(),
                cart.Items.Sum(x => x.Quantity),
                cart.Items.Sum(x => x.Quantity * x.PricePerUnit))
        { }
    }
}