using Application.DTO.OrderDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderPreviewDTO>> CreatePendingOrderAsync(Ulid UserId, CancellationToken token);
        Task<Result<OrderResponseDTO>> ChangeOrderStatusToSuccessAsync(Ulid orderId, CancellationToken token);
        Task<Result<OrderResponseDTO>> ChangeOrderStatusToFailAsync(Ulid orderId, CancellationToken token);
        Task<Result<List<OrderResponseDTO>>> GetMyOrdersAsync(Ulid UserId, CancellationToken token);
    }
}