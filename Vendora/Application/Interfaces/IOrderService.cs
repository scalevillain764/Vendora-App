using Application.DTO.OrderDTO;
using Domain.Carts;
using Domain.CartItems;
using Application.Result;
namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderPreviewDTO>> CreatePendingOrderAsync(Ulid UserId);
        Task<Result<OrderResponseDTO>> ChangeOrderStatusToSuccessAsync(Ulid orderId);
        Task<Result<OrderResponseDTO>> ChangeOrderStatusToFailAsync(Ulid orderId);
    }
}