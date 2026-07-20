namespace Application.DTO.PaymentDTO
{
    public record PaymentYOOKassaResponseDTO(
        string ExternalPaymentId,
        string PaymentUrl
        );
}