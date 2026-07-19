using Domain.OrderItems;
namespace Application.DTO.OrderDTO.OrderItemDTO
{
    public record OrderItemResponseDTO(
        Ulid ProductId,
        Ulid StoreId,
        string ProductName,
        decimal pricePerUnit,
        int Quantity
        )
    {
        public OrderItemResponseDTO(OrderItem orderItem) :
            this(orderItem.ProductId, orderItem.StoreId,
                orderItem.Product.Name, 
                orderItem.PricePerUnit, orderItem.Quantity)
        { }
    }
}