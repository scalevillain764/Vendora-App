using Domain.CartItems;
namespace Application.DTO.ProductDTO.CartDTO
{
    public record ProductCartCardResponseDTO(
        Ulid ProductId,
        Ulid CartItemId,
        string Name,
        decimal Price,
        string? ShortDescription,
        string? PreviewUrl,
        int Quantity
    )
    {
        public ProductCartCardResponseDTO(CartItem cartItem) :
            this(cartItem.Product.Id,
                cartItem.Id,
                cartItem.Product.Name,
                cartItem.PricePerUnit * cartItem.Quantity, 
                cartItem.Product.ShortDescription, 
                cartItem.Product.PreviewUrl,
                cartItem.Quantity)
        { }
    }
}