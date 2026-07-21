using Application.DTO.PaymentDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Orders;
using Domain.Transactions;
using Domain.Users;
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
        private async Task<bool> CompletePaymentAsync(Order order, User user, Transaction moneyTransaction)
        {
            user.Balance -= order.TotalPrice;

            var globalPayments = await _context.OrderItems
                .Where(x => x.OrderId == order.Id)
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
                await _orderService.ChangeOrderStatusToFailAsync(order.Id);
                moneyTransaction.Status = Transaction.PaymentStatus.Failed;
                return false;
            }

            foreach (var seller in sellers)
            {
                seller.Balance += globalPayments[seller.Id];
            }

            await _orderService.ChangeOrderStatusToSuccessAsync(order.Id);
            moneyTransaction.Status = Transaction.PaymentStatus.Success;

            return true;
        }

        public async Task<Result<PaymentResponseDTO>> ConfirmYooKassaPaymentAsync(Ulid UserId, PaymentYooKassaRequestDTO DTO)
        {
            var moneyTransaction = await _context.Transactions
                .Include(x => x.Order)
                    .FirstOrDefaultAsync(x => x.ExternalPaymentId == DTO.obj.Id);

            if (moneyTransaction == null)
                return Result<PaymentResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);

            var user = await _context.Users
                .FindAsync(UserId);

            if (user == null)
                return Result<PaymentResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            using var transaction = await _context.Database.BeginTransactionAsync();
           
            bool rez = await CompletePaymentAsync(moneyTransaction.Order, user, moneyTransaction);

            if(!rez)
                return Result<PaymentResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result<PaymentResponseDTO>.Success(new PaymentResponseDTO(moneyTransaction.OrderId, "OK"));
        }

        public async Task<Result<PaymentResponseDTO>> PayFromBalanceAsync(Ulid UserId, Ulid OrderId)
        {
            var order = await _context.Orders
                .FindAsync(OrderId);

            if (order == null)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentResponseDTO>.Error("Заказ не найден", ErrorType.NotFound);
            }
                
            if(order.UserId != UserId)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentResponseDTO>.Error("Это не ваш заказ", ErrorType.Forbidden);
            }
                
            var user = await _context.Users
                .FindAsync(UserId);

            if(user == null)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);
            }
              
            if (user.Balance < order.TotalPrice)
            {
                await _orderService.ChangeOrderStatusToFailAsync(OrderId);
                return Result<PaymentResponseDTO>.Error("Недостаточно средств", ErrorType.Validation);
            }

            var moneyTransaction = new Transaction(order.Id, null, order.TotalPrice, Transaction.PaymentMethod.Balance);
            _context.Transactions.Add(moneyTransaction);

            using var transaction = await _context.Database.BeginTransactionAsync();

            bool rez = await CompletePaymentAsync(moneyTransaction.Order, user, moneyTransaction);

            if(!rez)
                return Result<PaymentResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return Result<PaymentResponseDTO>.Success(new PaymentResponseDTO(OrderId, "OK"));
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
            _context.Transactions.Add(moneyTransaction);

            try
            {
                var response = await asyncClient.CreatePaymentAsync(newPayment);
                
                string paymentUrl = response.Confirmation.ConfirmationUrl;
                moneyTransaction.ExternalPaymentId = response.Id;

                await _context.SaveChangesAsync();
                return Result<PaymentYOOKassaResponseDTO>.Success(new PaymentYOOKassaResponseDTO(order.Id, response.Id, paymentUrl));
            }
            catch (Exception ex)
            {             
                return Result<PaymentYOOKassaResponseDTO>.Error("Что-то пошло не так", ErrorType.Conflict); // fix later
            }
        }
    }
}