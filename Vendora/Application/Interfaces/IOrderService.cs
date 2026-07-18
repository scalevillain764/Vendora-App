using Application.DTO.OrderDTO;
using Domain.Carts;
using Domain.CartItems;
using Application.Result;
namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderResponseDTO>> CreateOrder(Ulid UserId);
    }
}