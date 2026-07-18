using Application.DTO.OrderDTO;
using Domain.Carts;
using Domain.CartItems;
using Domain.Orders;
using Application.Result;
using Infrastructure.AppDbContexts;
using IOrderService = Application.Interfaces.IOrderService;
using Microsoft.EntityFrameworkCore;
using Domain.ErrorTypes;
using Domain.OrderItems;
namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<OrderResponseDTO>> CreateOrder(Ulid UserId)
        {
            var user = await _context.Users
                    .Include(x => x.Cart)
                        .ThenInclude(x => x.Items)
                            .ThenInclude(x => x.Product)
                                .FirstOrDefaultAsync(x => x.Id == UserId);

            if (user == null)
                return Result<OrderResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            var cart = user.Cart;

            if(cart == null)
                return Result<OrderResponseDTO>.Error("Корзина отсутствует", ErrorType.NotFound); // need to refactor and fix

            if(!cart.Items.Any())
                return Result<OrderResponseDTO>.Error("Корзина пуста", ErrorType.Conflict);

            decimal totalPrice = cart.Items.Sum(x => x.PricePerUnit * x.Quantity);

            var newOrder = new Order(UserId, totalPrice);

            var orderItems = cart.Items
                .Select(x => new OrderItem(newOrder, x))
                .ToList();

            newOrder.Items = orderItems;

            _context.
            await _context.SaveChangesAsync();

            var productNames = orderItems
                .Select(x => new KeyValuePair<string, int>(x.ProductName, x.Quantity))
                .ToDictionary();

            return Result<OrderResponseDTO>.Success(new OrderResponseDTO(newOrder, user, productNames));
        }
    }
}