namespace Application.DTO.AuthDTO.UserRegistrationResponseDTOS
{
    public record UserRegistrationResponseDTO
    (
        Ulid UserId,
        string Login
    );
}