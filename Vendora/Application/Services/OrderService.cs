using Application.DTO.OrderDTO;
using Application.DTO.OrderDTO.OrderItemDTO;
using Application.Result;
using Domain.CartItems;
using Domain.Carts;
using Domain.ErrorTypes;
using Domain.OrderItems;
using Domain.Orders;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IOrderService = Application.Interfaces.IOrderService;
namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        private async Task<Result<OrderResponseDTO>> ChangeOrderStatusAsync(Ulid orderId, Action<Order> action)
        {
            var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return Result<OrderResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);

            action(order);

            await _context.SaveChangesAsync();

            var orderItemResponseDTOS = order.Items
                .Select(x => new OrderItemResponseDTO(x))
                .ToList();

            return Result<OrderResponseDTO>.Success(new OrderResponseDTO(order, orderItemResponseDTOS));
        }
        public async Task<Result<OrderPreviewDTO>> CreatePendingOrderAsync(Ulid UserId)
        {       
            var cart = await _context.Carts
                 .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                        .ThenInclude(x => x.Store)
                        .FirstOrDefaultAsync(x => x.UserId == UserId);

            if(cart == null)
                return Result<OrderPreviewDTO>.Error("Корзина отсутствует", ErrorType.NotFound); // need to refactor and fix

            if(!cart.Items.Any())
                return Result<OrderPreviewDTO>.Error("Корзина пуста", ErrorType.Conflict);

            foreach (var item in cart.Items)
            {
                if (item.Product.Quantity < item.Quantity)
                    return Result<OrderPreviewDTO>.Error($"Недостаточно товара: {item.Product.Name}", ErrorType.Conflict);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            decimal totalPrice = cart.Items.Sum(x => x.PricePerUnit * x.Quantity);

            var newOrder = new Order(UserId, totalPrice);

            var orderItems = cart.Items
                .Select(x => new OrderItem(newOrder, x, x.Product.Store.SellerId))
                .ToList();

            newOrder.Items = orderItems;

            _context.Orders.Add(newOrder);
            _context.CartItems.RemoveRange(cart.Items);

            foreach (var item in cart.Items)
            {
                item.Product.Quantity -= item.Quantity;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var orderItemResponseDTOS = orderItems
                .Select(x => new OrderItemResponseDTO(x))
                .ToList();
         
            return Result<OrderPreviewDTO>.Success(new OrderPreviewDTO(newOrder, orderItemResponseDTOS));
        }

        public Task<Result<OrderResponseDTO>> ChangeOrderStatusToSuccessAsync(Ulid orderId)
            => ChangeOrderStatusAsync(orderId, order => order.Status = Order.OrderStatus.PaymentCompleted);

        public Task<Result<OrderResponseDTO>> ChangeOrderStatusToFailAsync(Ulid orderId)
              => ChangeOrderStatusAsync(orderId, order => order.Status = Order.OrderStatus.PaymentFailed);
    }
}