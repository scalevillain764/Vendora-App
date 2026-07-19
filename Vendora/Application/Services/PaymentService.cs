using Application.DTO.PaymentDTO;
using Application.Result;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Domain.ErrorTypes;
using IPaymentService = Application.Interfaces.IPaymentService;
namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly OrderService _orderService;
        public PaymentService(AppDbContext context, OrderService orderService)
        {
            _orderService = orderService;
            _context = context;
        }
        public async Task<Result<PaymentFromBalanceResponseDTO>> PayFromBalanceAsync(Ulid UserId, Ulid OrderId)
        {
            var order = await _context.Orders
                .FindAsync(OrderId);

            if (order == null)
            {
                await _orderService.FailPaymentAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);
            }
                
            if(order.UserId != UserId)
            {
                await _orderService.FailPaymentAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Это не ваш заказ", ErrorType.Forbidden);
            }
                
            var user = await _context.Users
                .FindAsync(UserId);

            if(user == null)
            {
                await _orderService.FailPaymentAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);
            }
              
            if (user.Balance < order.TotalPrice)
            {
                await _orderService.FailPaymentAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Недостаточно средств", ErrorType.Validation);
            }
                
            using var transaction = await _context.Database.BeginTransactionAsync();

            user.Balance -= order.TotalPrice;

            var globalPayments = await _context.OrderItems
                .Where(x => x.OrderId == OrderId)
                    .Select(x => new
                    {
                        SellerId = x.SellerId,
                        Sum = x.PricePerUnit * x.Quantity
                    })
                   .GroupBy(p => p.SellerId)
                        .ToDictionaryAsync(
                        group => group.Key,               
                        group => group.Sum(x => x.Sum)    
                        );

            var sellersId = globalPayments
                .Select(x => x.Key)
                .ToList();

            var sellers = await _context.Users
                .Where(u => sellersId.Contains(u.Id))
                .ToListAsync();

            if (globalPayments.Count != sellers.Count)
            {
                await _orderService.FailPaymentAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);
            }
                

            foreach(var seller in sellers)
            {
                seller.Balance += globalPayments[seller.Id];
            }

            await _orderService.ConfirmPaymentAsync(OrderId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<PaymentFromBalanceResponseDTO>.Success(new PaymentFromBalanceResponseDTO(OrderId, "OK"));
        }
    }
}