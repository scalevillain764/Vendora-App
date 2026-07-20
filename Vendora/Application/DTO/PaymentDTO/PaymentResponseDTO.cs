namespace Application.DTO.PaymentDTO
{
    public record PaymentResponseDTO(
        Ulid OrderId,
        string PaymentStatus
    );
}