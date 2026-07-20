using Application.DTO.PaymentDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentFromBalanceResponseDTO>> PayFromBalanceAsync(Ulid UserId, Ulid OrderId);
        Task<Result<PaymentYOOKassaResponseDTO>> PayFromYOOKassaAsync(Ulid UserId, Ulid OrderId);
    }
}