using Application.DTO.OrderDTO;
using Domain.Carts;
using Domain.CartItems;
using Application.Result;
namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<DTO.OrderDTO.OrderPreviewDTO>> CreatePendingOrderAsync(Ulid UserId);
        Task<Result<DTO.OrderDTO.OrderPreviewDTO>> ConfirmPaymentAsync(Ulid orderId);
    }
}