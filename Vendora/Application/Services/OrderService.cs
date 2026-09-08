using Application.DTO.OrderDTO;
using Application.DTO.OrderDTO.OrderItemDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.OrderItems;
using Domain.Orders;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IOrderService = Application.Interfaces.IOrderService;
using ICartService = Application.Interfaces.ICartService;
namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;
        public OrderService(AppDbContext context, ICartService cartService, ILogger<OrderService> logger)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;
        }
        private async Task<Result<OrderResponseDTO>> ChangeOrderStatusAsync(Ulid orderId, Action<Order> action, CancellationToken token)
        {
            var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId, token);

            if (order == null)
                return Result<OrderResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);

            action(order);

            await _context.SaveChangesAsync(token);

            var orderItemResponseDTOS = order.Items
                .Select(x => new OrderItemResponseDTO(x))
                .ToList();

            return Result<OrderResponseDTO>.Success(new OrderResponseDTO(order, orderItemResponseDTOS));
        }

        public async Task<Result<List<OrderResponseDTO>>> GetMyOrdersAsync(Ulid UserId, CancellationToken token) // pagination
        {
            var my_orders = await _context.Orders
                .Where(x => x.UserId == UserId)
                .Select(x => new OrderResponseDTO(x,
                    x.Items.Select(i => new OrderItemResponseDTO(i)).ToList()))
                .ToListAsync(token);

            return Result<List<OrderResponseDTO>>.Success(my_orders);
        }

        public async Task<Result<OrderPreviewDTO>> CreatePendingOrderAsync(Ulid userId, CancellationToken token)
        {
            var getMyCartRequest = await _cartService.GetMyCartAsync(userId, token);

            if (!getMyCartRequest.IsSuccess)
                return Result<OrderPreviewDTO>.Error(getMyCartRequest.ErrorMessage!, (ErrorType)getMyCartRequest.ErrorType!);

            var myCart = getMyCartRequest.data!;

            if (!myCart.cartItems.Any())
                return Result<OrderPreviewDTO>.Error("Корзина пуста", ErrorType.Conflict);

            var productIds = myCart.cartItems.Select(x => x.ProductId).ToList();

            var products = await _context.Products
                .Include(p => p.Store)
                .Include(p => p.Statistics)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(token);

            foreach (var cartItem in myCart.cartItems)
            {
                var dbProduct = products.FirstOrDefault(p => p.Id == cartItem.ProductId);

                if (dbProduct == null)
                    return Result<OrderPreviewDTO>.Error($"Товар с ID {cartItem.ProductId} не найден", ErrorType.NotFound);

                if (dbProduct.Quantity < cartItem.Quantity)
                    return Result<OrderPreviewDTO>.Error($"Недостаточно товара '{dbProduct.Name}' на складе. Доступно: {dbProduct.Quantity}, в корзине: {cartItem.Quantity}", ErrorType.Conflict);
            }

            using var transaction = await _context.Database.BeginTransactionAsync(token);

            decimal totalPrice = myCart.cartItems.Sum(x => x.PricePerUnit * x.Quantity);
            var newOrder = new Order(userId, totalPrice);

            var orderItems = new List<OrderItem>();

            foreach (var cartItem in myCart.cartItems)
            {
                var dbProduct = products.First(p => p.Id == cartItem.ProductId);

                var orderItem = new OrderItem(
                    orderId: newOrder.Id,
                    sellerId: dbProduct.Store.SellerId,
                    storeId: dbProduct.StoreId,
                    productId: dbProduct.Id,
                    productName: dbProduct.Name,
                    productPrice: dbProduct.Price,
                    productQuantity: cartItem.Quantity
                );

                orderItems.Add(orderItem);

                dbProduct.Quantity -= cartItem.Quantity;

                if (dbProduct.Statistics != null)
                    dbProduct.Statistics.OrdersCount++;
            }

            newOrder.Items = orderItems;
            _context.Orders.Add(newOrder);

            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            await _cartService.ClearCartAsync(userId);

            var orderItemResponseDTOs = orderItems
                .Select(x => new OrderItemResponseDTO(x))
                .ToList();

            _logger.LogInformation("User №{UserId} created order №{OrderId}", userId, newOrder.Id);

            return Result<OrderPreviewDTO>.Success(new OrderPreviewDTO(newOrder, orderItemResponseDTOs));
        }

        public Task<Result<OrderResponseDTO>> ChangeOrderStatusToSuccessAsync(Ulid orderId, CancellationToken token)
            => ChangeOrderStatusAsync(orderId, order => order.Status = Order.OrderStatus.PaymentCompleted, token);

        public Task<Result<OrderResponseDTO>> ChangeOrderStatusToFailAsync(Ulid orderId, CancellationToken token)
              => ChangeOrderStatusAsync(orderId, order => order.Status = Order.OrderStatus.PaymentFailed, token);
    }
}