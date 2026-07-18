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
        public async Task<Result<OrderPreviewDTO>> CreatePendingOrderAsync(Ulid UserId)
        {       
            var cart = await _context.Carts
                 .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
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
                .Select(x => new OrderItem(newOrder, x))
                .ToList();

            newOrder.Items = orderItems;

            _context.Orders.Add(newOrder);
            _context.CartItems.RemoveRange(cart.Items);

            foreach (var item in cart.Items)
            {
                item.Product.Quantity -= item.Quantity;
            }

            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();

            var orderItemResponseDTOS = orderItems
                .Select(x => new OrderItemResponseDTO(x))
                .ToList();
         
            return Result<OrderPreviewDTO>.Success(new OrderPreviewDTO(newOrder.Id, UserId, totalPrice, newOrder.Status.ToString(), orderItemResponseDTOS));
        }


    }
}