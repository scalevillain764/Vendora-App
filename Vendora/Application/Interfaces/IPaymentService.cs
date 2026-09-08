using Application.DTO.PaymentDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentResponseDTO>> PayFromBalanceAsync(Ulid UserId, Ulid OrderId, CancellationToken token);
        Task<Result<PaymentYOOKassaResponseDTO>> PayFromYOOKassaAsync(Ulid UserId, Ulid OrderId, CancellationToken token);
        Task<Result<PaymentResponseDTO>> ConfirmYooKassaPaymentAsync(Ulid UserId, PaymentYooKassaRequestDTO DTO, CancellationToken token);
    }
}