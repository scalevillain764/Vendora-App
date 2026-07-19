namespace Application.DTO.PaymentDTO
{
    public record PaymentFromBalanceResponseDTO(
        Ulid OrderId,
        string PaymentStatus
    );
}