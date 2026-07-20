using Application.DTO.PaymentDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Transactions;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using IOrderService = Application.Interfaces.IOrderService;
using IPaymentService = Application.Interfaces.IPaymentService;
namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        public PaymentService(AppDbContext context, IOrderService orderService)
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
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);
            }
                
            if(order.UserId != UserId)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Это не ваш заказ", ErrorType.Forbidden);
            }
                
            var user = await _context.Users
                .FindAsync(UserId);

            if(user == null)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);
            }
              
            if (user.Balance < order.TotalPrice)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentFromBalanceResponseDTO>.Error("Недостаточно средств", ErrorType.Validation);
            }
                
            using var transaction = await _context.Database.BeginTransactionAsync();

            user.Balance -= order.TotalPrice;

            var moneyTransaction = new Transaction(order.Id, null, order.TotalPrice, Transaction.PaymentMethod.Balance);
            _context.Transactions.Add(moneyTransaction);

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
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                moneyTransaction.Status = Transaction.PaymentStatus.Failed;
                return Result<PaymentFromBalanceResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);
            }
                
            foreach(var seller in sellers)
            {
                seller.Balance += globalPayments[seller.Id];
            }

            await _orderService.ChangeOrderStatusToFailAsync(OrderId);
            moneyTransaction.Status = Transaction.PaymentStatus.Success;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return Result<PaymentFromBalanceResponseDTO>.Success(new PaymentFromBalanceResponseDTO(OrderId, "OK"));
        }

        public async Task<Result<PaymentYOOKassaResponseDTO>> PayFromYOOKassaAsync(Ulid UserId, Ulid OrderId)
        {
            var order = await _context.Orders
                .FindAsync(OrderId);

            if (order == null)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentYOOKassaResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);
            }

            if (order.UserId != UserId)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentYOOKassaResponseDTO>.Error("Это не ваш заказ", ErrorType.Forbidden);
            }

            var user = await _context.Users
                .FindAsync(UserId);

            if (user == null)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentYOOKassaResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);
            }

            if (user.Balance < order.TotalPrice)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentYOOKassaResponseDTO>.Error("Недостаточно средств", ErrorType.Validation);
            }

            var client = new Client(shopId: $"{Environment.GetEnvironmentVariable("YOOKASSA_SHOP_ID")}", secretKey: $"{Environment.GetEnvironmentVariable("YOOKASSA_API_KEY")}");
            var asyncClient = client.MakeAsync();

            var newPayment = new NewPayment
            {
                Amount = new Amount
                {
                    Value = order.TotalPrice,
                    Currency = "RUB" // fix later
                },
                Capture = true,
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = $"{Environment.GetEnvironmentVariable("YOOKASSA_BACK_URL")}"
                },
                Description = $"Payment №{order.Id}"
            };

            var moneyTransaction = new Transaction(OrderId, null, order.TotalPrice, Transaction.PaymentMethod.YOOKassa);

            try
            {
                var response = await asyncClient.CreatePaymentAsync(newPayment);

                string paymentUrl = newPayment.Confirmation.ConfirmationUrl;
                moneyTransaction.ExternalPaymentId = response.Id;

                await _context.SaveChangesAsync();
                return Result<PaymentYOOKassaResponseDTO>.Success(new PaymentYOOKassaResponseDTO(response.Id, paymentUrl));
            }
            catch (Exception ex)
            {             
                return Result<PaymentYOOKassaResponseDTO>.Error("Что-то пошло не так", ErrorType.Conflict); // fix later
            }
        }
    }
}