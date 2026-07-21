namespace Application.DTO.PaymentDTO
{
    public record PaymentYOOKassaResponseDTO(
        Ulid OrderId,
        string ExternalPaymentId,
        string PaymentUrl
        );
}